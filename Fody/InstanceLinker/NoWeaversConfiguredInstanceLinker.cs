using MethodTimer;

public partial class Processor
{
    [Time]
    public void ConfigureWhenNoWeaversFound()
    {
        if (!FoundWeaverProjectFile)
        {
            return;
        }
        if (WeaverProjectUsed)
        {
            return;
        }
        var weaverProjectContainsType = WeaverProjectContainsType("ModuleWeaver");
        if (weaverProjectContainsType)
        {
            Logger.LogInfo("Found 'ModuleWeaver' in project 'Weavers' so will run that one.");
            var weaverEntry = new WeaverEntry
                                  {
                                      AssemblyPath = WeaverAssemblyPath,
                                      TypeName = "ModuleWeaver",
                                      AssemblyName = "Weavers"
                                  };
            Weavers.Add(weaverEntry);
        }
    }
}