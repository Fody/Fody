using System.Collections.Generic;
using Fody;

public class AddinDirectoriesFinder
{
    public ILogger Logger;
    public WeavingTask WeavingTask;

    public List<string> FindAddinDirectories()
    {
        var nugetPackagePathFinder = new NugetPackagePathFinder
            {
                Logger = Logger,
                WeavingTask = WeavingTask
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
                WeavingTask = WeavingTask
            };
        configDirectoryFinder.Execute();
        var toolsDirectoryFinder = new ToolsDirectoryFinder
            {
                AddinDirectories = addinDirectories,
                Logger = Logger,
                WeavingTask = WeavingTask
            };
        toolsDirectoryFinder.Execute();
        addinDirectories.Execute();
        return addinDirectories.SearchPaths;
    }
}