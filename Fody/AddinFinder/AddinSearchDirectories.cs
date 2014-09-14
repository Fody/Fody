using System.Collections.Generic;
using System.IO;

public partial class AddinFinder
{
    public void FindAddinDirectories()
    {
        AddNugetDirectoryFromConvention();
        AddNugetDirectoryFromNugetConfig();
        AddCurrentFodyDirectoryToAddinSearch();
        AddToolsSolutionDirectoryToAddinSearch();
    }

    public void AddToolsSolutionDirectoryToAddinSearch()
    {
        Logger.LogDebug(string.Format("SolutionDirectoryPath: {0}", SolutionDirectoryPath));
        var solutionDirToolsDirectory = Path.Combine(SolutionDirectoryPath, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            Logger.LogDebug(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", solutionDirToolsDirectory));
            return;
        }

        Logger.LogDebug(string.Format("Adding weaver dlls from '{0}'.", solutionDirToolsDirectory));
        AddFiles(Directory.EnumerateFiles(solutionDirToolsDirectory, "*.Fody.dll", SearchOption.AllDirectories));
    }

    public void AddCurrentFodyDirectoryToAddinSearch()
    {
        Logger.LogDebug(string.Format("SolutionDirectoryPath: {0}", SolutionDirectoryPath));
        var fodyParentDirectory = Directory.GetParent(AssemblyLocation.CurrentDirectory).FullName;

        if (!Directory.Exists(fodyParentDirectory))
        {
            Logger.LogDebug(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", fodyParentDirectory));
            return;
        }

        AddWeaversFromDir(fodyParentDirectory);
    }


    public void AddNugetDirectoryFromNugetConfig()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(SolutionDirectoryPath);
        if (packagesPathFromConfig == null)
        {
            Logger.LogDebug("Could not find packages dir from nuget config.");
            return;
        }
        if (!Directory.Exists(packagesPathFromConfig))
        {
            Logger.LogDebug(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", packagesPathFromConfig));
            return;
        }
        AddWeaversFromDir(packagesPathFromConfig);
    }

    public void AddNugetDirectoryFromConvention()
    {
        var solutionPackages = Path.Combine(SolutionDirectoryPath, "Packages");
        if (!Directory.Exists(solutionPackages))
        {
            Logger.LogDebug(string.Format("Skipped scanning '{0}' for weavers since it doesn't exist.", solutionPackages));
            return;
        }
        AddWeaversFromDir(solutionPackages);
    }

    void AddWeaversFromDir(string directory)
    {
        Logger.LogDebug(string.Format("Adding weaver dlls from '{0}'.", directory));
        foreach (var packageDir in Directory.GetDirectories(directory, "*.Fody*"))
        {
            AddFiles(Directory.EnumerateFiles(packageDir, "*.Fody.dll"));
        }
    }

    void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            Logger.LogDebug(string.Format("Fody weaver file added '{0}'", file));
            FodyFiles.Add(file);
        }
    }


    public ILogger Logger;
    public string SolutionDirectoryPath;
}