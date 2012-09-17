using System.Collections.Generic;
using System.Linq;

public partial class AddinFinder
{
    public List<string> AddinSearchPaths = new List<string>();

    public void FindAddinDirectories()
    {
        FindNugetPackagePath();
        AddNugetDirectoryToAddinSearch();
        AddToolsSolutionDirectoryToAddinSearch();
        AddToolsAssemblyLocationToAddinSearch();
        LogAddinSearchPaths();
        CacheAllFodyAddinDlls();
    }

    public ILogger Logger;
    public string SolutionDir;

    public void LogAddinSearchPaths()
    {
        AddinSearchPaths = AddinSearchPaths.Distinct().ToList();
        foreach (var searchPath in AddinSearchPaths)
        {
            Logger.LogInfo(string.Format("Directory added to addin search paths '{0}'.", searchPath));
        }
    }

}