using System;
using System.Collections.Generic;
using System.Security.Permissions;

public class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string AssemblyFilePath { get; set; }
    public string ProjectFilePath { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public string References { get; set; }
    public List<WeaverEntry> Weavers { get; set; }
    public string KeyFilePath { get; set; }
    public ILogger Logger { get; set; }
    public string IntermediateDirectoryPath { get; set; }

    public void Execute()
    {
        try
        {
            //  AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveAssembly(args);
            var referenceFinder = new AssemblyReferenceFinder(this, Logger);
            referenceFinder.Execute();
            var assemblyResolver = new AssemblyResolver(referenceFinder);
            var reader = new ModuleReader
                             {
                                 AssemblyResolver = assemblyResolver,
                                 InnerWeaver = this,
                                 Logger = Logger
                             };
            reader.Execute();

            var weaverInitialiser = new WeaverInitialiser
                                        {
                                            ModuleDefinition = reader.ModuleDefinition,
                                            AssemblyResolver = assemblyResolver,
                                            InnerWeaver = this,
                                            Logger = Logger
                                        };
            weaverInitialiser.Execute();

            var weaverRunner = new ModuleWeaverRunner
                                   {
                                       Logger = Logger,
                                   };
            Logger.LogInfo("");
            foreach (var weaverInstance in weaverInitialiser.WeaverInstances)
            {
                var weaverName = ObjectTypeName.GetAssemblyName(weaverInstance);
                Logger.SetCurrentWeaverName(weaverName);
                try
                {
                    weaverRunner.Execute(weaverInstance);
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

            var keyFinder = new StrongNameKeyFinder
                {
                    InnerWeaver = this,
                    Logger = Logger, 
                    ModuleDefinition = reader.ModuleDefinition
                };
            keyFinder.Execute();
            var moduleWriter = new ModuleWriter
                                   {
                                       InnerWeaver = this,
                                       Logger = Logger,
                                       ModuleDefinition = reader.ModuleDefinition,
                                       StrongNameKeyFinder = keyFinder,
                                   };
            moduleWriter.Execute();

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
