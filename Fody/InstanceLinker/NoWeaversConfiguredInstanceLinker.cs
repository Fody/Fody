public class NoWeaversConfiguredInstanceLinker
{
    public ProjectWeaversReader ProjectWeaversReader;
    public WeaverProjectContainsWeaverChecker WeaverProjectContainsWeaverChecker;
    public BuildLogger Logger;
    public WeaverProjectFileFinder WeaverProjectFileFinder;

    public void Execute()
    {
        if (!WeaverProjectFileFinder.Found)
        {
            return;
        }
        if (WeaverProjectContainsWeaverChecker.WeaverProjectUsed)
        {
            return;
        }
        var weaverProjectContainsType = WeaverProjectContainsWeaverChecker.WeaverProjectContainsType("ModuleWeaver");
        if (weaverProjectContainsType)
        {
            Logger.LogInfo("Found 'ModuleWeaver' in project 'Weavers' so will run that one.");
            var weaverEntry = new WeaverEntry
                                  {
                                      AssemblyPath = WeaverProjectFileFinder.WeaverAssemblyPath,
                                      TypeName = "ModuleWeaver"
                                  };
            ProjectWeaversReader.Weavers.Add(weaverEntry);
        }
    }
}