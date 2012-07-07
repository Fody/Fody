public class WeaversConfiguredInstanceLinker
{
    public ProjectWeaversReader ProjectWeaversReader;
    public WeaverAssemblyPathFinder WeaverAssemblyPathFinder;
    public WeaverProjectContainsWeaverChecker WeaverProjectContainsWeaverChecker;
    public WeaverProjectFileFinder WeaverProjectFileFinder;

    public void Execute()
    {

        foreach (var weaverConfig in ProjectWeaversReader.Weavers)
        {
            ProcessConfig(weaverConfig);
        }

    }

    public void ProcessConfig(WeaverEntry weaverConfig)
    {
        var weaverProjectContains = WeaverProjectContainsWeaverChecker.WeaverProjectContainsType(weaverConfig.AssemblyName);
        if (weaverProjectContains)
        {
            weaverConfig.AssemblyPath = WeaverProjectFileFinder.WeaverAssemblyPath;
            weaverConfig.TypeName = weaverConfig.AssemblyName;
            return;
        }

        var assemblyPath = WeaverAssemblyPathFinder.FindAssemblyPath(weaverConfig.AssemblyName);
        if (assemblyPath == null)
        {
            throw new WeavingException(string.Format("Could not find a weaver named '{0}'.", weaverConfig.AssemblyName));
        }
        weaverConfig.AssemblyPath = assemblyPath;
        weaverConfig.TypeName = "ModuleWeaver";
    }
}