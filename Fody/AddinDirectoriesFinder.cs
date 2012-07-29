using System.Collections.Generic;

public class AddinDirectoriesFinder
{
    public ILogger Logger;
    public string SolutionDir;
    public string AddinSearchPaths;


    public List<string> FindAddinDirectories()
    {
        var nugetPackagePathFinder = new NugetPackagePathFinder
            {
                Logger = Logger,
                SolutionDir = SolutionDir
            };
        nugetPackagePathFinder.Execute();
        var addinDirectories = new AddinDirectories
            {
                Logger = Logger
            };
        var nugetDirectoryFinder = new NugetDirectoryFinder
            {
                Logger = Logger,
                AddinDirectories = addinDirectories,
                NugetPackagePathFinder = nugetPackagePathFinder
            };
        nugetDirectoryFinder.Execute();
        var configDirectoryFinder = new ConfigDirectoryFinder
            {
                AddinDirectories = addinDirectories,
                Logger = Logger,
                SolutionDir = SolutionDir,
                AddinSearchPaths = AddinSearchPaths
            };
        configDirectoryFinder.Execute();
        var toolsDirectoryFinder = new ToolsDirectoryFinder
            {
                AddinDirectories = addinDirectories,
                Logger = Logger,
                SolutionDir = SolutionDir
            };
        toolsDirectoryFinder.Execute();
        addinDirectories.Execute();
        return addinDirectories.SearchPaths;
    }
}