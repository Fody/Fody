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
    public ILogger Logger { get; set; }
    public string IntermediateDirectoryPath { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }
    public List<string> DefineConstants { get; set; }
    bool cancelRequested;
    List<WeaverHolder> weaverInstances = new List<WeaverHolder>();
    Action cancelDelegate;

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        foreach (var weaverPath in Weavers.Select(x => x.AssemblyPath))
        {
            var directoryName = Path.GetDirectoryName(weaverPath);
            var assemblyFileName = new AssemblyName(args.Name).Name + ".dll";
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
                var message = string.Format("Failed to load '{0}'. Going to swallow and continue to let other AssemblyResolve events to attempt to resolve. Exception:{1}", assemblyPath, exception);
                Logger.LogWarning(message);
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
            InitialiseWeavers();
            ExecuteWeavers();
            AddProcessedFlag();
            FindStrongNameKey();
            WriteModule();
            ExecuteAfterWeavers();
            DisposeWeavers();

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

    public void Cancel()
    {
        cancelRequested = true;
        var action = cancelDelegate;
        if (action != null)
        {
            action();
        }
    }

    void AddProcessedFlag()
    {
        ModuleDefinition.Types.Add(new TypeDefinition(null, "ProcessedByFody", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
    }

    void InitialiseWeavers()
    {
        foreach (var weaverConfig in Weavers)
        {
            if (cancelRequested)
            {
                return;
            }
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
                if (weaver.WeaverDelegate.Cancel != null)
                {
                    cancelDelegate = () => weaver.WeaverDelegate.Cancel(weaver.Instance);
                }
                Logger.SetCurrentWeaverName(weaver.Config.AssemblyName);
                var startNew = Stopwatch.StartNew();
                Logger.LogDebug("  Executing Weaver ");
                weaver.WeaverDelegate.Execute(weaver.Instance);
                var finishedMessage = string.Format("  Finished '{0}' in {1}ms {2}", weaver.Config.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                Logger.LogDebug(finishedMessage);
            }
            finally
            {
                cancelDelegate = null;
                Logger.ClearWeaverName();
            }
        }
    }

    void ExecuteAfterWeavers()
    {
        foreach (var weaver in weaverInstances
            .Where(_ => _.WeaverDelegate.AfterWeavingExecute != null))
        {
            if (cancelRequested)
            {
                return;
            }
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
        //Disconnects the remoting channel(s) of this object and all nested objects.
        RemotingServices.Disconnect(this);
    }

}
