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
using FieldAttributes = Mono.Cecil.FieldAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

public partial class InnerWeaver :
    MarshalByRefObject,
    IInnerWeaver
{
    static InnerWeaver()
    {
        StaticAssemblyResolve.Init();
    }
    public string ProjectDirectoryPath { get; set; }
    public string ProjectFilePath { get; set; }
    public string DocumentationFilePath { get; set; }
    public string AssemblyFilePath { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public string References { get; set; }
    public List<WeaverEntry> Weavers { get; set; }
    public string KeyFilePath { get; set; }
    public bool SignAssembly { get; set; }
    public ILogger Logger { get; set; }
    public string IntermediateDirectoryPath { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }
    public List<string> DefineConstants { get; set; }
    public DebugSymbolsType DebugSymbols { get; set; }
    bool cancelRequested;
    List<WeaverHolder> weaverInstances = new List<WeaverHolder>();
    Action cancelDelegate;
    public AssemblyResolver assemblyResolver;

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name;

        foreach (var weaverPath in Weavers.Select(x => x.AssemblyPath))
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

    public TypeCache TypeCache;
    public void Execute()
    {
        ResolveEventHandler assemblyResolve = CurrentDomain_AssemblyResolve;
        try
        {
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolve;
            SplitUpReferences();
            GetSymbolProviders();
            assemblyResolver = new AssemblyResolver(Logger, SplitReferences);
            ReadModule();
            if (ModuleDefinition.Types.Any(x => x.Name == "ProcessedByFody"))
            {
                Logger.LogWarning($"The assembly has already been processed by Fody. Weaving aborted. Path: {AssemblyFilePath} ");
                return;
            }
            TypeCache = new TypeCache(assemblyResolver.Resolve);
            InitialiseWeavers();

            TypeCache.BuildAssembliesToScan(weaverInstances.Select(x => x.Instance));
            InitialiseTypeSystem();
            ExecuteWeavers();
            AddWeavingInfo();
            FindStrongNameKey();
            WriteModule();
            ModuleDefinition?.Dispose();
            CleanupTempSymbolsAndAssembly();
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
            CleanupTempSymbolsAndAssembly();
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
            weaverInstances.Add(weaverHolder);
        }
    }

    WeaverHolder InitialiseWeaver(WeaverEntry weaverConfig)
    {
        Logger.LogDebug($"Weaver '{weaverConfig.AssemblyPath}'.");
        Logger.LogDebug("  Initializing weaver");
        var assembly = LoadWeaverAssembly(weaverConfig.AssemblyPath);
        var weaverType = assembly.FindType(weaverConfig.TypeName);

        var delegateHolder = weaverType.GetDelegateHolderFromCache();
        var weaverInstance = delegateHolder();
        var weaverHolder = new WeaverHolder
        {
            Instance = weaverInstance,
            Config = weaverConfig
        };

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
                Logger.LogInfo("  Executing Weaver ");
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
                catch (Exception exception)
                {
                    throw new Exception($"Failed to execute weaver {weaver.Config.AssemblyPath}", exception);
                }

                var finishedMessage = $"  Finished '{weaver.Config.ElementName}' in {startNew.ElapsedMilliseconds}ms {Environment.NewLine}";
                Logger.LogDebug(finishedMessage);

                ReferenceCleaner.CleanReferences(ModuleDefinition, weaver.Instance, Logger.LogDebug);
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

        Logger.LogDebug("  Adding weaving info");
        var startNew = Stopwatch.StartNew();

        const TypeAttributes typeAttributes = TypeAttributes.NotPublic | TypeAttributes.Class;
        var typeDefinition = new TypeDefinition(null, "ProcessedByFody", typeAttributes, TypeSystem.ObjectReference);
        ModuleDefinition.Types.Add(typeDefinition);

        AddVersionField(typeof(IInnerWeaver).Assembly, "FodyVersion", typeDefinition);

        foreach (var weaver in weaverInstances)
        {
            var configAssembly = weaver.Instance.GetType().Assembly;
            var name = weaver.Config.ElementName.Replace(".", string.Empty);
            AddVersionField(configAssembly, name, typeDefinition);
        }

        var finishedMessage = $"  Finished in {startNew.ElapsedMilliseconds}ms {Environment.NewLine}";
        Logger.LogDebug(finishedMessage);
    }

    void AddVersionField(Assembly assembly, string name, TypeDefinition typeDefinition)
    {
        var weaverVersion = "0.0.0.0";
        var attrs = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute));
        var fileVersionAttribute = (AssemblyFileVersionAttribute)attrs.FirstOrDefault();
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
                Logger.LogDebug("  Executing After Weaver");
                weaver.Instance.AfterWeaving();
                var finishedMessage = $"  Finished '{weaver.Config.ElementName}' in {stopwatch.ElapsedMilliseconds}ms {Environment.NewLine}";
                Logger.LogDebug(finishedMessage);
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
            .Select(x => x.Instance)
            .OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }

    public sealed override object InitializeLifetimeService()
    {
        // Returning null designates an infinite non-expiring lease.
        // We must therefore ensure that RemotingServices.Disconnect() is called when
        // it's no longer needed otherwise there will be a memory leak.
        return null;
    }

    public void Dispose()
    {
#if NET472
        //Disconnects the remoting channel(s) of this object and all nested objects.
        RemotingServices.Disconnect(this);
#endif
    }
}