using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    public List<string> AddinSearchPaths = new List<string>();

    public void FindAddinDirectories()
    {
        AddNugetDirectoryToAddinSearch();
        AddToolsSolutionDirectoryToAddinSearch();
        AddToolsAssemblyLocationToAddinSearch();
        LogAddinSearchPaths();
        CacheAllFodyAddinDlls();
    }

    public void AddToolsSolutionDirectoryToAddinSearch()
    {
        var solutionDirToolsDirectory = Path.GetFullPath(Path.Combine(SolutionDirectoryPath, "Tools"));
        AddinSearchPaths.Add(solutionDirToolsDirectory);
    }

    public void AddNugetDirectoryToAddinSearch()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(SolutionDirectoryPath);
        if (packagesPathFromConfig != null)
        {
            AddinSearchPaths.Add(packagesPathFromConfig);
        }
        AddinSearchPaths.Add(Path.Combine(SolutionDirectoryPath, "Packages"));
    }


    public ILogger Logger;
    public string SolutionDirectoryPath;

    public void LogAddinSearchPaths()
    {
        AddinSearchPaths = AddinSearchPaths.Distinct().ToList();
        foreach (var searchPath in AddinSearchPaths.ToList())
        {
            if (Directory.Exists(searchPath))
            {
                Logger.LogInfo(string.Format("Directory added to addin search paths '{0}'.", searchPath));
            }
            else
            {
                AddinSearchPaths.Remove(searchPath);
                Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", searchPath));
            }

        }
    }

}