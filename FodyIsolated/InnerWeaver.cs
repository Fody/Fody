using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using Mono.Cecil;
using TypeAttributes = Mono.Cecil.TypeAttributes;

public partial class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string ProjectDirectoryPath { get; set; }
    public string AssemblyFilePath { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public string References { get; set; }
    public List<WeaverEntry> Weavers { get; set; }
    public string KeyFilePath { get; set; }
    public bool SignAssembly { get; set; }
    public bool VerifyAssembly { get; set; }
    public ILogger Logger { get; set; }
    public string IntermediateDirectoryPath { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }
    public List<string> DefineConstants { get; set; }

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        foreach (var weaverPath in Weavers.Select(x => x.AssemblyPath))
        {
            var directoryName = Path.GetDirectoryName(weaverPath);
            var assemblyFileName = new AssemblyName(args.Name).Name + ".dll";
            string assemblyPath = Path.Combine(directoryName, assemblyFileName);
            if (File.Exists(assemblyPath))
            {
                return LoadFromFile(assemblyPath);
            }
        }
        return null;
    }

    public void Execute()
    {
        ResolveEventHandler assemblyResolve = CurrentDomain_AssemblyResolve;
        try
        {
            SplitUpReferences();
            GetSymbolProviders();
            ReadModule();
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolve;
            var weaverInstances = new List<WeaverHolder>();
            InitialiseWeavers(weaverInstances);
            ExecuteWeavers(weaverInstances);
            AddProcessedFlag();
            FindStrongNameKey();
            WriteModule();
            ExecuteAfterWeavers(weaverInstances);
            DisposeWeavers(weaverInstances);

            if (weaverInstances
                .Any(_ => _.WeaverDelegate.AfterWeavingExecute != null))
            {
                ReadModule();
                WriteModule();
            }
        }
        catch (Exception exception)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolve;
            Logger.LogException(exception);
        }

    }

    void AddProcessedFlag()
    {
        ModuleDefinition.Types.Add(new TypeDefinition(null, "ProcessedByFody", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
    }

    void InitialiseWeavers(List<WeaverHolder> weaverInstances)
    {
        foreach (var weaverConfig in Weavers)
        {
            Logger.LogDebug(string.Format("Weaver '{0}'.", weaverConfig.AssemblyPath));
            Logger.LogDebug("  Initializing weaver");
            var assembly = LoadAssembly(weaverConfig.AssemblyPath);

            var weaverType = assembly.FindType(weaverConfig.TypeName);

            var delegateHolder = weaverType.GetDelegateHolderFromCache();
            var weaverInstance = delegateHolder.ConstructInstance();
            var weaverHolder = new WeaverHolder
                               {
                                   Instance = weaverInstance,
                                   WeaverDelegate = delegateHolder,
                                   Config = weaverConfig
                               };
            weaverInstances.Add(weaverHolder);

            SetProperties(weaverConfig, weaverInstance, delegateHolder);
        }
    }

    void ExecuteWeavers(List<WeaverHolder> weaverInstances)
    {
        foreach (var weaver in weaverInstances)
        {
            try
            {
                Logger.SetCurrentWeaverName(weaver.Config.AssemblyName);
                var startNew = Stopwatch.StartNew();
                Logger.LogInfo(string.Format("  Executing Weaver {0}", weaver.Config.AssemblyName));
                weaver.WeaverDelegate.Execute(weaver.Instance);
                var finishedMessage = string.Format("  Finished '{0}' in {1}ms {2}", weaver.Config.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                Logger.LogDebug(finishedMessage);
            }
            finally
            {
                Logger.ClearWeaverName();
            }
        }
    }

    void ExecuteAfterWeavers(List<WeaverHolder> weaverInstances)
    {
        foreach (var weaver in weaverInstances
            .Where(_ => _.WeaverDelegate.AfterWeavingExecute != null))
        {
            try
            {
                Logger.SetCurrentWeaverName(weaver.Config.AssemblyName);
                var startNew = Stopwatch.StartNew();
                Logger.LogDebug("  Executing After Weaver");
                weaver.WeaverDelegate.AfterWeavingExecute(weaver.Instance);
                var finishedMessage = string.Format("  Finished '{0}' in {1}ms {2}", weaver.Config.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                Logger.LogDebug(finishedMessage);
            }
            finally
            {
                Logger.ClearWeaverName();
            }
        }
    }

    static void DisposeWeavers(List<WeaverHolder> weaverInstances)
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
        //Disconnects the remoting channel(s) of this object and all nested objects.
        RemotingServices.Disconnect(this);
    }

}
