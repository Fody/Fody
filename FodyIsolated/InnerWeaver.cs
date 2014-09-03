using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using Mono.Cecil;

public partial class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string ProjectDirectoryPath { get; set; }
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

    public void Execute()
    {
        try
        {
            SplitUpReferences();
            GetSymbolProviders();
            ReadModule();
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
            Logger.LogInfo(string.Format("Weaver '{0}'.", weaverConfig.AssemblyPath));
            Logger.LogInfo("  Initializing weaver");
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
                Logger.LogInfo("  Executing Weaver ");
                weaver.WeaverDelegate.Execute(weaver.Instance);
                var finishedMessage = string.Format("  Finished '{0}' in {1}ms {2}", weaver.Config.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                Logger.LogInfo(finishedMessage);
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
                Logger.LogInfo("  Executing After Weaver");
                weaver.WeaverDelegate.AfterWeavingExecute(weaver.Instance);
                var finishedMessage = string.Format("  Finished '{0}' in {1}ms {2}", weaver.Config.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                Logger.LogInfo(finishedMessage);
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
