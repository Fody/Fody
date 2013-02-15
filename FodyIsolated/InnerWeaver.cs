using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using FodyIsolated;

public partial class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string AssemblyFilePath { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public bool DebugLoggingEnabled { get; set; }
    public string References { get; set; }
    public List<WeaverEntry> Weavers { get; set; }
    public string KeyFilePath { get; set; }
    public bool SignAssembly { get; set; }
    public ILogger Logger { get; set; }
    public string IntermediateDirectoryPath { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }

    public void Execute()
    {
        if (DebugLoggingEnabled)
        {
            MethodTimeLogger.LogDebug = s => Logger.LogInfo(s);
        }
        try
        {
            SplitUpReferences();
            GetSymbolProviders();
            ReadModule();

            foreach (var weaverConfig in Weavers)
            {

                Logger.LogInfo(string.Format("Loading weaver '{0}'.", weaverConfig.AssemblyPath));
                var assembly = LoadAssembly(weaverConfig.AssemblyPath);

                var weaverType = assembly.FindType(weaverConfig.TypeName);

                object weaverInstance = null;
                try
                {
                    weaverInstance = weaverType.ConstructInstance();

                    var delegateHolder = GetDelegateHolderFromCache(weaverType);

                    SetProperties(weaverConfig, weaverInstance, delegateHolder);

                    Logger.SetCurrentWeaverName(weaverConfig.AssemblyName);
                    try
                    {
                        var startNew = Stopwatch.StartNew();
                        Logger.LogInfo(string.Format("Executing Weaver '{0}'.", weaverConfig.AssemblyName));
                        delegateHolder.Execute(weaverInstance);
                        var message = string.Format("Finished '{0}' in {1}ms {2}", weaverConfig.AssemblyName, startNew.ElapsedMilliseconds, Environment.NewLine);
                        Logger.LogInfo(message);
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
                finally
                {
                    if (weaverInstance != null)
                    {
                        var disposable = weaverInstance as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }

            FindStrongNameKey();
            WriteModule();
        }
        catch (Exception exception)
        {
            Logger.LogError(exception.ToFriendlyString());
        }
    }

    public static WeaverDelegateHolder GetDelegateHolderFromCache(Type weaverType)
    {
        WeaverDelegateHolder delegateHolder;
        if (!weaverDelegates.TryGetValue(weaverType.TypeHandle, out delegateHolder))
        {
            weaverDelegates[weaverType.TypeHandle] = delegateHolder = BuildDelegateHolder(weaverType);
        }
        return delegateHolder;
    }

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}
