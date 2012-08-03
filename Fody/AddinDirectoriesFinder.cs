
public partial class Processor
{

    public void FindAddinDirectories()
    {
        FindNugetPackagePath();
        AddNugetDirectoryToAddinSearch();
        AddMsBuildConfigToAddinSearch();
        AddToolsDirectoryToAddinSearch();
        LogAddinSearchPaths();
    }
}