using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET472
using System.Runtime.Remoting;
#endif
using Fody;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

public partial class InnerWeaver :
    MarshalByRefObject,
    IInnerWeaver
{
    public string ProjectDirectoryPath { get; set; } = null!;
    public string ProjectFilePath { get; set; } = null!;
    public string? DocumentationFilePath { get; set; }
    public string AssemblyFilePath { get; set; } = null!;
    public string SolutionDirectoryPath { get; set; } = null!;
    public string References { get; set; } = null!;
    public List<WeaverEntry> Weavers { get; set; } = null!;
    public string? KeyFilePath { get; set; }
    public bool SignAssembly { get; set; }
    public bool DelaySign { get; set; }
    public ILogger Logger { get; set; } = null!;
    public string IntermediateDirectoryPath { get; set; } = null!;
    public List<string> ReferenceCopyLocalPaths { get; set; } = null!;
    public List<string> RuntimeCopyLocalPaths { get; set; } = null!;
    public List<string> DefineConstants { get; set; } = null!;
#if (NETSTANDARD)
    public IsolatedAssemblyLoadContext LoadContext { get; set; } = null!;
#endif
    bool cancelRequested;
    List<WeaverHolder> weaverInstances = new();
    Action? cancelDelegate;
    public IAssemblyResolver assemblyResolver = null!;

    Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name;
        if (assemblyName == "FodyHelpers")
        {
            return typeof(BaseModuleWeaver).Assembly;
        }

        if (assemblyName == "Mono.Cecil")
        {
            return typeof(ModuleDefinition).Assembly;
        }

        if (assemblyName == "Mono.Cecil.Rocks")
        {
            return typeof(MethodBodyRocks).Assembly;
        }

        if (assemblyName == "Mono.Cecil.Pdb")
        {
            return typeof(PdbReaderProvider).Assembly;
        }

        foreach (var weaverPath in Weavers.Select(_ => _.AssemblyPath))
        {
            var directoryName = Path.GetDirectoryName(weaverPath);
            var assemblyFileName = $"{assemblyName}.dll";
            var assemblyPath = Path.Combine(directoryName, assemblyFileName);
            if (!File.Exists(assemblyPath))
            {
                continue;
            }

            try
            {
                return LoadFromFile(assemblyPath);
            }
            catch (Exception exception)
            {
                var message = $"Failed to load '{assemblyPath}'. Going to swallow and continue to let other AssemblyResolve events to attempt to resolve. Exception:{exception}";
                Logger.LogWarning(message);
            }
        }

        return null;
    }

    public TypeCache TypeCache = null!;
    public void Execute()
    {
        ResolveEventHandler assemblyResolve = CurrentDomain_AssemblyResolve;
        try
        {
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolve;
            SplitUpReferences();
            assemblyResolver = new AssemblyResolver(Logger, SplitReferences);
            ReadModule();
            var weavingInfoClassName = GetWeavingInfoClassName();
            if (ModuleDefinition.Types.Any(_ => _.Name == weavingInfoClassName))
            {
                Logger.LogWarning($"The assembly has already been processed by Fody. Weaving aborted. Path: {AssemblyFilePath}");
                return;
            }
            TypeCache = new(assemblyResolver.Resolve);
            InitialiseWeavers();
            ValidatePackageReferenceSettings(weaverInstances, Logger);
            TypeCache.BuildAssembliesToScan(weaverInstances.Select(_ => _.Instance));
            InitialiseTypeSystem();
            ExecuteWeavers();
            AddWeavingInfo();
            FindStrongNameKey();
            WriteModule();
            ModuleDefinition?.Dispose();
            ExecuteAfterWeavers();
            DisposeWeavers();
        }
        catch (Exception exception)
        {
            Logger.LogException(exception);
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolve;
            ModuleDefinition?.Dispose();
            assemblyResolver?.Dispose();
        }
    }

    public void Cancel()
    {
        cancelRequested = true;
        cancelDelegate?.Invoke();
    }

    void InitialiseWeavers()
    {
        foreach (var weaverConfig in Weavers)
        {
            if (cancelRequested)
            {
                return;
            }

            var weaverHolder = InitialiseWeaver(weaverConfig);
            if (weaverHolder != null)
            {
                weaverInstances.Add(weaverHolder);
            }
        }
    }

    WeaverHolder? InitialiseWeaver(WeaverEntry weaverConfig)
    {
        Logger.LogDebug($"Weaver '{weaverConfig.AssemblyPath}'.");
        Logger.LogDebug("  Initializing weaver");
        var assembly = LoadWeaverAssembly(weaverConfig.AssemblyPath);
        var weaverType = assembly.FindType(weaverConfig.TypeName);

        if (weaverType == null)
        {
            Logger.LogError($"Could not find weaver type {weaverConfig.TypeName} in {weaverConfig.WeaverName}");
            return null;
        }

        var delegateHolder = weaverType.GetDelegateHolderFromCache();
        var weaverInstance = delegateHolder();
        var weaverHolder = new WeaverHolder(weaverInstance, weaverConfig);

        if (FodyVersion.WeaverRequiresUpdate(assembly, out var referencedVersion))
        {
            Logger.LogWarning($"Weavers should reference at least the current major version of Fody (version {FodyVersion.Major}). The weaver in {assembly.GetName().Name} references version {referencedVersion}. This may result in incompatibilities at build time such as MissingMethodException being thrown.", "FodyVersionMismatch");
            weaverHolder.IsUsingOldFodyVersion = true;
        }

        weaverHolder.FodyVersion = referencedVersion;

        SetProperties(weaverConfig, weaverInstance);
        return weaverHolder;
    }

    void ExecuteWeavers()
    {
        foreach (var weaver in weaverInstances)
        {
            if (cancelRequested)
            {
                return;
            }

            try
            {
                cancelDelegate = weaver.Instance.Cancel;

                Logger.SetCurrentWeaverName(weaver.Config.ElementName);
                var startNew = Stopwatch.StartNew();
                var assembly = weaver.Instance.GetType().Assembly;
                Logger.LogInfo($"Executing weaver {weaver.Config.ElementName} v{assembly.GetVersion()}");
                Logger.LogDebug($"Configuration source: {weaver.Config.ConfigurationSource}");
                try
                {
                    weaver.Instance.Execute();
                }
                catch (WeavingException)
                {
                    throw;
                }
                catch (MissingMemberException exception) when (weaver.IsUsingOldFodyVersion)
                {
                    throw new WeavingException($"Failed to execute weaver {weaver.Config.AssemblyPath} due to a MissingMemberException. Message: {exception.Message}. This is likely due to the weaver referencing an old version ({weaver.FodyVersion}) of Fody.");
                }
                catch (FileNotFoundException exception) when (exception.Message.Contains(nameof(ValueTuple)))
                {
                    throw new($@"Failed to execute weaver {weaver.Config.AssemblyPath} due to a failure to load ValueTuple.
This is a known issue with in dotnet (https://github.com/dotnet/runtime/issues/27533).
The recommended work around is to avoid using ValueTuple inside a weaver.", exception);
                }
                catch (Exception exception)
                {
                    throw new($"Failed to execute weaver {weaver.Config.AssemblyPath}", exception);
                }

                Logger.LogDebug($"Finished '{weaver.Config.ElementName}' in {startNew.ElapsedMilliseconds}ms");

                ReferenceCleaner.CleanReferences(ModuleDefinition, weaver.Instance, ReferenceCopyLocalPaths, RuntimeCopyLocalPaths, Logger.LogDebug);
            }
            finally
            {
                cancelDelegate = null;
                Logger.ClearWeaverName();
            }
        }
    }

    void AddWeavingInfo()
    {
        if (cancelRequested)
        {
            return;
        }

        Logger.LogDebug("Adding weaving info");
        var startNew = Stopwatch.StartNew();

        const TypeAttributes typeAttributes = TypeAttributes.NotPublic | TypeAttributes.Class;
        var typeDefinition = new TypeDefinition(null, GetWeavingInfoClassName(), typeAttributes, TypeSystem.ObjectReference);
        ModuleDefinition.Types.Add(typeDefinition);

        AddVersionField(typeof(IInnerWeaver).Assembly, "FodyVersion", typeDefinition);

        foreach (var weaver in weaverInstances)
        {
            var configAssembly = weaver.Instance.GetType().Assembly;
            var name = weaver.Config.ElementName.Replace(".", string.Empty);
            AddVersionField(configAssembly, name, typeDefinition);
        }

        Logger.LogDebug($"Finished in {startNew.ElapsedMilliseconds}ms");
    }

    string GetWeavingInfoClassName()
    {
        var classPrefix = ModuleDefinition.Assembly.Name.Name.Replace(".", "");
        return $"{classPrefix}_ProcessedByFody";
    }

    void AddVersionField(Assembly assembly, string name, TypeDefinition typeDefinition)
    {
        var weaverVersion = "0.0.0.0";
        var attrs = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute));
        var fileVersionAttribute = (AssemblyFileVersionAttribute?)attrs.FirstOrDefault();
        if (fileVersionAttribute != null)
        {
            weaverVersion = fileVersionAttribute.Version;
        }

        const FieldAttributes fieldAttributes = FieldAttributes.Assembly |
                                                FieldAttributes.Literal |
                                                FieldAttributes.Static |
                                                FieldAttributes.HasDefault;
        var field = new FieldDefinition(name, fieldAttributes, TypeSystem.StringReference)
        {
            Constant = weaverVersion
        };

        typeDefinition.Fields.Add(field);
    }

    void ExecuteAfterWeavers()
    {
        foreach (var weaver in weaverInstances)
        {
            if (cancelRequested)
            {
                return;
            }

            try
            {
                Logger.SetCurrentWeaverName(weaver.Config.ElementName);
                var stopwatch = Stopwatch.StartNew();
                Logger.LogDebug("Executing After Weaver");
                weaver.Instance.AfterWeaving();
                Logger.LogDebug($"Finished '{weaver.Config.ElementName}' in {stopwatch.ElapsedMilliseconds}ms");
            }
            finally
            {
                Logger.ClearWeaverName();
            }
        }
    }

    void DisposeWeavers()
    {
        foreach (var disposable in weaverInstances
            .Select(_ => _.Instance)
            // ReSharper disable once SuspiciousTypeConversion.Global
            .OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }

    public sealed override object? InitializeLifetimeService() =>
        // Returning null designates an infinite non-expiring lease.
        // We must therefore ensure that RemotingServices.Disconnect() is called when
        // it's no longer needed otherwise there will be a memory leak.
        null;

    public void Dispose()
    {
#if NET472
        //Disconnects the remoting channel(s) of this object and all nested objects.
        RemotingServices.Disconnect(this);
#endif
    }
}
