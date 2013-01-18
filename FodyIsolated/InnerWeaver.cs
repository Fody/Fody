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

    public void Execute()
    {
        try
        {
            //  AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveAssembly(args);
            SplitUpReferences();
            
            GetSymbolProviders();
            ReadModule();

            SetWeaverProperties();

            Logger.LogInfo("");
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
        catch (Exception exception)
        {
            Logger.LogError(exception.ToFriendlyString());
        }
    }

    //Assembly ResolveAssembly(ResolveEventArgs resolveEventArgs)
    //{
    //    var replace = resolveEventArgs.RequestingAssembly.GetName().Name.Replace(".Fody", string.Empty);
    //    var weaverEntry = Weavers.FirstOrDefault(x => x.AssemblyName == replace);
    //    if (weaverEntry == null)
    //    {
    //        return null;
    //    }
    //    var directoryName = Path.GetDirectoryName(weaverEntry.AssemblyPath);

    //    var dllPathToLoad = Path.Combine(directoryName, new AssemblyName(resolveEventArgs.Name).Name+".dll");
    //    if (File.Exists(dllPathToLoad))
    //    {
    //        var readAllBytes = File.ReadAllBytes(dllPathToLoad);
    //        return Assembly.Load(readAllBytes);
    //    }
    //    return null;
    //}


    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}
