using System;
using System.Collections.Generic;
using System.Security.Permissions;

public partial class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string AssemblyFilePath { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public string References { get; set; }
    public List<WeaverEntry> Weavers { get; set; }
    public string KeyFilePath { get; set; }
    public bool SignAssembly { get; set; }
    public ILogger Logger { get; set; }
    public string IntermediateDirectoryPath { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }

    public void Execute()
    {
        try
        {
            SplitUpReferences();
            GetSymbolProviders();
            ReadModule();
            SetWeaverProperties();

            Logger.LogInfo("");
            try
            {

                foreach (var weaverInstance in WeaverInstances)
                {
                    var weaverName = ObjectTypeName.GetAssemblyName(weaverInstance);
                    Logger.SetCurrentWeaverName(weaverName);
                    try
                    {
                        RunWeaver(weaverInstance);
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
            }
            finally
            {
                DisposeWeavers();
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
