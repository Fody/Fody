using System;
using System.Collections.Generic;
using System.Security.Permissions;

public class InnerWeaver : MarshalByRefObject, IInnerWeaver
{
    public string AssemblyPath { get; set; }
    public string References { get; set; }
    public List<WeaverEntry> Weavers { get; set; }
    public string KeyFilePath { get; set; }
    public ILogger Logger { get; set; }
    public string IntermediateDir { get; set; }

    public void Execute()
    {
        try
        {

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
                                            InnerWeavingTask = this,
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
                try
                {
                    weaverRunner.Execute(weaverInstance);
                }
                catch (Exception exception)
                {
                    Logger.LogError(string.Format("{0}: {1}", weaverName, exception.ToFriendlyString()));
                    return;
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



    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}
