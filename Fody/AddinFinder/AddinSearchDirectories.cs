using System.Collections.Generic;
using System.IO;
using MethodTimer;

public partial class AddinFinder
{
    [Time]
    public void FindAddinDirectories()
    {
        AddNugetDirectoryToAddinSearch();
        AddToolsSolutionDirectoryToAddinSearch();
    }

    [Time]
    public void AddToolsSolutionDirectoryToAddinSearch()
    {
        Logger.LogInfo(string.Format("SolutionDirectoryPath: {0}", SolutionDirectoryPath));
        var solutionDirToolsDirectory = Path.Combine(SolutionDirectoryPath, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            Logger.LogInfo(string.Format("'{0}' directory doesn't exist.", solutionDirToolsDirectory));
            return;
        }

        Logger.LogInfo(string.Format("Adding weaver dlls from '{0}'.", solutionDirToolsDirectory));
        AddFiles(Directory.EnumerateFiles(solutionDirToolsDirectory, "*.Fody.dll", SearchOption.AllDirectories));
    }

    void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            Logger.LogInfo(string.Format("Fody weaver file added '{0}'", file));
            FodyFiles.Add(file);
        }
    }

    [Time]
    public void AddNugetDirectoryToAddinSearch()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(SolutionDirectoryPath);
        if (packagesPathFromConfig != null)
        {
            Logger.LogInfo(string.Format("Adding weaver dlls from '{0}'.", packagesPathFromConfig));
            foreach (var packageDir in Directory.GetDirectories(packagesPathFromConfig))
            {
                AddFiles(Directory.EnumerateFiles(packageDir, "*.Fody.dll"));
            }
        }

        var solutionPackages = Path.Combine(SolutionDirectoryPath, "Packages");
        if (!Directory.Exists(solutionPackages))
        {
            Logger.LogInfo(string.Format("'{0}' directory doesn't exist.", solutionPackages));
            return;
        }

        Logger.LogInfo(string.Format("Adding weaver dlls from '{0}'.", solutionPackages));
        foreach (var packageDir in Directory.GetDirectories(solutionPackages))
        {
            AddFiles(Directory.EnumerateFiles(packageDir, "*.Fody.dll"));
        }
    }

    public ILogger Logger;
    public string SolutionDirectoryPath;
}