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
            throw new WeavingException(string.Format("Could not find a weaver named '{0}'.", weaverConfig.AssemblyName));
        }
        weaverConfig.AssemblyPath = assemblyPath;
        weaverConfig.TypeName = "ModuleWeaver";
    }
}