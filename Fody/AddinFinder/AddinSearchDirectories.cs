using System.Collections.Generic;
using System.IO;
using MethodTimer;

public partial class AddinFinder
{
    [Time]
    public void FindAddinDirectories()
    {
        AddNugetDirectoryFromConvention();
        AddNugetDirectoryFromNugetConfig();
        AddCurrentFodyDirectoryToAddinSearch();
        AddToolsSolutionDirectoryToAddinSearch();
    }

    [Time]
    public void AddToolsSolutionDirectoryToAddinSearch()
    {
        Logger.LogInfo(string.Format("SolutionDirectoryPath: {0}", SolutionDirectoryPath));
        var solutionDirToolsDirectory = Path.Combine(SolutionDirectoryPath, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            Logger.LogInfo(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", solutionDirToolsDirectory));
            return;
        }

        Logger.LogInfo(string.Format("Adding weaver dlls from '{0}'.", solutionDirToolsDirectory));
        AddFiles(Directory.EnumerateFiles(solutionDirToolsDirectory, "*.Fody.dll", SearchOption.AllDirectories));
    }

    [Time]
    public void AddCurrentFodyDirectoryToAddinSearch()
    {
        Logger.LogInfo(string.Format("SolutionDirectoryPath: {0}", SolutionDirectoryPath));
        var fodyParentDirectory = Directory.GetParent(AssemblyLocation.CurrentDirectory()).FullName;

        if (!Directory.Exists(fodyParentDirectory))
        {
            Logger.LogInfo(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", fodyParentDirectory));
            return;
        }

        AddWeaversFromDir(fodyParentDirectory);
    }


    [Time]
    public void AddNugetDirectoryFromNugetConfig()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(SolutionDirectoryPath);
        if (packagesPathFromConfig == null)
        {
            Logger.LogInfo("Could not find packages dir from nuget config.");
            return;
        }
        if (!Directory.Exists(packagesPathFromConfig))
        {
            Logger.LogInfo(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", packagesPathFromConfig));
            return;
        }
        AddWeaversFromDir(packagesPathFromConfig);
    }

    [Time]
    public void AddNugetDirectoryFromConvention()
    {
        var solutionPackages = Path.Combine(SolutionDirectoryPath, "Packages");
        if (!Directory.Exists(solutionPackages))
        {
            Logger.LogInfo(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", solutionPackages));
            return;
        }
        AddWeaversFromDir(solutionPackages);
    }

    void AddWeaversFromDir(string directory)
    {
        Logger.LogInfo(string.Format("Adding weaver dlls from '{0}'.", directory));
        foreach (var packageDir in Directory.GetDirectories(directory, "*.Fody*"))
        {
            AddFiles(Directory.EnumerateFiles(packageDir, "*.Fody.dll"));
        }
    }

    void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            Logger.LogInfo(string.Format("Fody weaver file added '{0}'", file));
            FodyFiles.Add(file);
        }
    }


    public ILogger Logger;
    public string SolutionDirectoryPath;
}