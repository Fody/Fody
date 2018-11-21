using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET46
using System.Runtime.Remoting;
#endif
using Fody;
using Mono.Cecil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;
#pragma warning disable 618

public partial class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string ProjectDirectoryPath { get; set; }
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

        if (assemblyName == "Mono.Cecil.Mdb")
        {
            return typeof(MdbReaderProvider).Assembly;
        }

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

#pragma warning disable 618
    public TypeCache TypeCache;
    public void Execute()
    {
        ResolveEventHandler assemblyResolve = CurrentDomain_AssemblyResolve;
        try
        {
            SplitUpReferences();
            GetSymbolProviders();
            assemblyResolver = new AssemblyResolver(Logger, SplitReferences);
            ReadModule();
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolve;
            TypeCache = new TypeCache(assemblyResolver.Resolve);
            InitialiseWeavers();

            TypeCache.BuildAssembliesToScan(weaverInstances.Select(x=>x.Instance));
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
            AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolve;
            Logger.LogException(exception);
        }
        finally
        {
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
            weaverInstances.Add(weaverHolder);

            SetProperties(weaverConfig, weaverInstance);
        }
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

                Logger.SetCurrentWeaverName(weaver.Config.AssemblyName);
                var startNew = Stopwatch.StartNew();
                Logger.LogDebug("  Executing Weaver ");
                try
                {
                    weaver.Instance.Execute();
                }
                catch (Exception exception)
                {
                    throw new Exception($"Failed to execute weaver {weaver.Config.AssemblyPath}", exception);
                }
                var finishedMessage = $"  Finished '{weaver.Config.AssemblyName}' in {startNew.ElapsedMilliseconds}ms {Environment.NewLine}";
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

        var typeAttributes = TypeAttributes.NotPublic |
                             TypeAttributes.Class;
        var typeDefinition = new TypeDefinition(null, "ProcessedByFody", typeAttributes, TypeSystem.ObjectReference);
        ModuleDefinition.Types.Add(typeDefinition);

        AddVersionField(typeof(IInnerWeaver).Assembly, "FodyVersion", typeDefinition);

        foreach (var weaver in weaverInstances)
        {
            var configAssembly = weaver.Instance.GetType().Assembly;
            var name = weaver.Config.AssemblyName.Replace(".", string.Empty);
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

        var fieldAttributes = FieldAttributes.Assembly |
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
                Logger.SetCurrentWeaverName(weaver.Config.AssemblyName);
                var stopwatch = Stopwatch.StartNew();
                Logger.LogDebug("  Executing After Weaver");
                weaver.Instance.AfterWeaving();
                var finishedMessage = $"  Finished '{weaver.Config.AssemblyName}' in {stopwatch.ElapsedMilliseconds}ms {Environment.NewLine}";
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
#if NET46
        //Disconnects the remoting channel(s) of this object and all nested objects.
        RemotingServices.Disconnect(this);
#endif
    }
}