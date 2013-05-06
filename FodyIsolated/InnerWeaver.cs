using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;

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
            var disposableWeavers = new List<IDisposable>();
            foreach (var weaverConfig in Weavers)
            {
                var startNew = Stopwatch.StartNew();
                Logger.LogInfo(string.Format("Weaver '{0}'.", weaverConfig.AssemblyPath));
                Logger.LogInfo("\tInitializing weaver");
                var assembly = LoadAssembly(weaverConfig.AssemblyPath);

                var weaverType = assembly.FindType(weaverConfig.TypeName);

                var delegateHolder = weaverType.GetDelegateHolderFromCache();
				var weaverInstance = delegateHolder.ConstructInstance();
                var disposable = weaverInstance as IDisposable;
                if (disposable != null)
                {
                    disposableWeavers.Add(disposable);
                }

                SetProperties(weaverConfig, weaverInstance, delegateHolder);

                Logger.SetCurrentWeaverName(weaverConfig.AssemblyName);
                try
                {
                    Logger.LogInfo("\tExecuting Weaver ");
                    delegateHolder.Execute(weaverInstance);
                    var finishedMessage = string.Format("\tFinished '{0}' in {1}ms {2}", weaverConfig.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                    Logger.LogInfo(finishedMessage);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception.ToFriendlyString());
                    return;
                }
                finally
                {
                    Logger.ClearWeaverName();
                }

            }

            FindStrongNameKey();
            WriteModule();
            foreach (var disposable in disposableWeavers)
            {
                disposable.Dispose();
            }
        }
        catch (Exception exception)
        {
            Logger.LogError(exception.ToFriendlyString());
        }
    }


    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}
