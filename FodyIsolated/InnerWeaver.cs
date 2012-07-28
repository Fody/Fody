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
        string weaverName = null;
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
                                       WeaverInitialiser = weaverInitialiser,
                                       SetCurrentWeaverName = s => weaverName = s
                                   };
            weaverRunner.Execute();
            weaverName = null;

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
            Logger.LogError(weaverName ,exception.ToFriendlyString());
        }
    }



    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}
