using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    public void FindAddinDirectories()
    {
        if (PackageDefinitions == null)
        {
            AddNugetDirectoryFromConvention();
            AddNugetDirectoryFromNugetConfig();
            AddCurrentFodyDirectoryToAddinSearch();
            AddToolsSolutionDirectoryToAddinSearch();
            AddNuGetPackageRootToAddinSearch();
        }
        else
        {
            // each PackageDefinition will be of the format C:\...\packages\propertychanging.fody\1.28.0
            // so must be a Contains(.fody)
            foreach (var directory in PackageDefinitions.Where(x => x.ToLowerInvariant().Contains(".fody")))
            {
                AddFiles(Directory.EnumerateFiles(directory, "*.Fody.dll"));
            }
            AddToolsSolutionDirectoryToAddinSearch();
        }
    }

    void AddNuGetPackageRootToAddinSearch()
    {
        if (NuGetPackageRoot == null)
        {
            return;
        }
        if (!Directory.Exists(NuGetPackageRoot))
        {
            Logger.LogDebug($"Skipped scanning '{NuGetPackageRoot}' for weavers since it doesn't exist.");
        }
        AddFiles(ScanNuGetPackageRoot(NuGetPackageRoot));
    }

    public static IEnumerable<string> ScanNuGetPackageRoot(string nuGetPackageRoot)
    {
        var fodyWeaverDirectories = Directory.EnumerateDirectories(nuGetPackageRoot, "*.?ody")
                                       .Where(dir => dir.ToLowerInvariant().EndsWith(".fody"));

        foreach (var packageDirectory in fodyWeaverDirectories)
        {
            var packageName = Path.GetFileName(packageDirectory);
            foreach (var versionDirectory in Directory.EnumerateDirectories(packageDirectory))
            {
                var lowercasePackageName = $"{packageName?.ToLowerInvariant()}.dll";
                var files = Directory.EnumerateFiles(versionDirectory);
                var assembly = files.FirstOrDefault(file => Path.GetFileName(file)?.ToLowerInvariant() == lowercasePackageName);

                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }
    }

    public void AddToolsSolutionDirectoryToAddinSearch()
    {
        Logger.LogDebug($"SolutionDirectoryPath: {SolutionDirectoryPath}");
        var solutionDirToolsDirectory = Path.Combine(SolutionDirectoryPath, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            Logger.LogDebug($"Skipped scanning '{solutionDirToolsDirectory}' for weavers since it doesn't exist.");
            return;
        }

        Logger.LogDebug($"Adding weaver dlls from '{solutionDirToolsDirectory}'.");
        AddFiles(Directory.EnumerateFiles(solutionDirToolsDirectory, "*.Fody.dll", SearchOption.AllDirectories));
    }

    public void AddCurrentFodyDirectoryToAddinSearch()
    {
        Logger.LogDebug($"SolutionDirectoryPath: {SolutionDirectoryPath}");
        var fodyParentDirectory = Directory.GetParent(AssemblyLocation.CurrentDirectory).FullName;

        if (!Directory.Exists(fodyParentDirectory))
        {
            Logger.LogDebug($"Skipped scanning '{fodyParentDirectory}' for weavers since it doesn't exist.");
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
            Logger.LogDebug($"Skipped scanning '{packagesPathFromConfig}' for weavers since it doesn't exist.");
            return;
        }
        AddWeaversFromDir(packagesPathFromConfig);
    }

    public void AddNugetDirectoryFromConvention()
    {
        var solutionPackages = Path.Combine(SolutionDirectoryPath, "Packages");
        if (!Directory.Exists(solutionPackages))
        {
            Logger.LogDebug($"Skipped scanning '{solutionPackages}' for weavers since it doesn't exist.");
            return;
        }
        AddWeaversFromDir(solutionPackages);
    }

    void AddWeaversFromDir(string directory)
    {
        Logger.LogDebug($"Adding weaver dlls from '{directory}'.");
        foreach (var packageDir in Directory.GetDirectories(directory, "*.Fody*"))
        {
            AddFiles(Directory.EnumerateFiles(packageDir, "*.Fody.dll"));
        }
    }

    void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            Logger.LogDebug($"Fody weaver file added '{file}'");
            FodyFiles.Add(file);
        }
    }


    public ILogger Logger;
    public string SolutionDirectoryPath;
    public string NuGetPackageRoot;
    public List<string> PackageDefinitions;

}