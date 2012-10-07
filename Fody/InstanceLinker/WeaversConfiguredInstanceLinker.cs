using System;

public partial class Processor
{

    public void ConfigureWhenWeaversFound()
    {

        foreach (var weaverConfig in Weavers)
        {
            ProcessConfig(weaverConfig);
        }

    }

    public void ProcessConfig(WeaverEntry weaverConfig)
    {
        //support for diff names weavers when "In solution weaving"
        var weaverProjectContains = WeaverProjectContainsType(weaverConfig.AssemblyName);
        if (weaverProjectContains)
        {
            weaverConfig.AssemblyPath = WeaverAssemblyPath;
            weaverConfig.TypeName = weaverConfig.AssemblyName;
            return;
        }

        var assemblyPath = FindAssemblyPath(weaverConfig.AssemblyName);
        if (assemblyPath == null)
        {
            var searchPaths = string.Join(Environment.NewLine, addinFinder.AddinSearchPaths);
            var message = string.Format("Could not find a weaver named '{0}'. Tried:{1}{2}.", weaverConfig.AssemblyName, Environment.NewLine, searchPaths);
            throw new WeavingException(message);
        }
        weaverConfig.AssemblyPath = assemblyPath;
        weaverConfig.TypeName = "ModuleWeaver";
    }
}