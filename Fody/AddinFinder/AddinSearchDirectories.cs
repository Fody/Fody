using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public partial class AddinFinder
{
    readonly Action<string> log;
    readonly string solutionDirectory;
    readonly string msBuildTaskDirectory;
    readonly string nuGetPackageRoot;
    readonly string weaverProbingPaths;

    IDictionary<string, string> weaversFromProbingPaths;

    public AddinFinder(Action<string> log, string solutionDirectory, string msBuildTaskDirectory, string nuGetPackageRoot, string weaverProbingPaths)
    {
        this.log = log;
        this.solutionDirectory = solutionDirectory;
        this.msBuildTaskDirectory = msBuildTaskDirectory;
        this.nuGetPackageRoot = nuGetPackageRoot;
        this.weaverProbingPaths = weaverProbingPaths;
    }

    public void FindAddinDirectories()
    {
        weaversFromProbingPaths = AddinProbingPathFinder.Lookup(weaverProbingPaths, log);
    }

    void FindAddinDirectoriesLegacy()
    {
        log("FindAddinDirectories (Legacy):");

        AddNugetDirectoryFromConvention();
        AddNugetDirectoryFromNugetConfig();
        AddFromMsBuildDirectory();
        AddToolsSolutionDirectoryToAddinSearch();
        AddNuGetPackageRootToAddinSearch();
    }

    void AddNuGetPackageRootToAddinSearch()
    {
        if (nuGetPackageRoot == null)
        {
            log("  Skipped NuGetPackageRoot since it is not defined.");
            return;
        }
        if (!Directory.Exists(nuGetPackageRoot))
        {
            log($"  Skipped NuGetPackageRoot '{nuGetPackageRoot}' since it doesn't exist.");
            return;
        }
        log($"  Scanning NuGetPackageRoot '{nuGetPackageRoot}'.");

        AddFiles(ScanDirectoryForPackages(nuGetPackageRoot));
    }

    public static IEnumerable<string> ScanDirectoryForPackages(string directory)
    {
        return AddOldStyleDirectories(directory)
            .Concat(AddNewOrPaketStyleDirectories(directory).Where(x => x != null));
    }

    static IEnumerable<string> AddNewOrPaketStyleDirectories(string directory)
    {
        foreach (var packageDirectory in DirectoryEx.EnumerateDirectoriesEndsWith(directory, ".fody"))
        {
            var packageName = Path.GetFileName(packageDirectory);

            var newStyleVersionDirectories = Directory.EnumerateDirectories(packageDirectory)
                .Where(dir => Regex.IsMatch(Path.GetFileName(dir), @"^[0-9]+\.[0-9]+"))
                .ToList();

            if (newStyleVersionDirectories.Any())
            {
                foreach (var versionDirectory in newStyleVersionDirectories)
                {
                    yield return GetAssemblyFromNugetDir(versionDirectory, packageName);
                }
            }
            else
            {
                yield return GetAssemblyFromNugetDir(packageDirectory, packageName);
            }
        }
    }

    static IEnumerable<string> AddOldStyleDirectories(string directory)
    {
        foreach (var versionDirectory in DirectoryEx.EnumerateDirectoriesContains(directory, ".fody."))
        {
            var fileName = Path.GetFileName(versionDirectory);
            var index = fileName.IndexOf(".fody.", StringComparison.OrdinalIgnoreCase);
            var packageName = fileName.Substring(0, index + 5);
            yield return GetAssemblyFromNugetDir(versionDirectory, packageName);
        }
    }

    static string GetAssemblyFromNugetDir(string versionDir, string packageName)
    {
#if (NETSTANDARD2_0)
        var specificDir = Path.Combine(versionDir, "netstandardweaver");
#endif
#if (NET46)
        var specificDir = Path.Combine(versionDir, "netclassicweaver");
#endif

        return GetAssemblyFromDir(specificDir, packageName)
               ?? GetAssemblyFromDir(versionDir, packageName);
    }

    static string GetAssemblyFromDir(string dir, string packageName)
    {
        if (!Directory.Exists(dir))
        {
            return null;
        }

        return Directory.GetFiles(dir)
            .Select(x => new FileInfo(x))
            .FirstOrDefault(x => x.Name.Equals($"{packageName}.dll", StringComparison.OrdinalIgnoreCase))?.FullName;
    }

    void AddToolsSolutionDirectoryToAddinSearch()
    {
        var solutionDirToolsDirectory = Path.Combine(solutionDirectory, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            log($"  Skipped scanning '{solutionDirToolsDirectory}' since it doesn't exist.");
            return;
        }

        log($"  Scanning SolutionDir/Tools directory convention: '{solutionDirToolsDirectory}'.");
        AddFiles(DirectoryEx.EnumerateFilesEndsWith(solutionDirToolsDirectory, ".Fody.dll", SearchOption.AllDirectories));
    }

    void AddFromMsBuildDirectory()
    {
        var fromMsBuildThisFileDirectory = Path.GetFullPath(Path.Combine(msBuildTaskDirectory, "../../../"));
        if (!Directory.Exists(fromMsBuildThisFileDirectory))
        {
            log($"  Skipped scanning '{fromMsBuildThisFileDirectory}' since it doesn't exist.");
            return;
        }

        log($"  Scanning the MsBuildThisFileDirectory parent: {fromMsBuildThisFileDirectory}'.");
        AddFiles(ScanDirectoryForPackages(fromMsBuildThisFileDirectory));
    }

    void AddNugetDirectoryFromNugetConfig()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDirectory);
        if (packagesPathFromConfig == null)
        {
            log("  Skipped directory from Nuget Config since it could not be derived.");
            return;
        }
        if (!Directory.Exists(packagesPathFromConfig))
        {
            log($"  Skipped directory from Nuget Config '{packagesPathFromConfig}' since it doesn't exist.");
            return;
        }
        log($"  Scanning directory from Nuget Config: {packagesPathFromConfig}'.");
        AddFiles(ScanDirectoryForPackages(packagesPathFromConfig));
    }

    void AddNugetDirectoryFromConvention()
    {
        foreach (var solutionPackages in DirectoryEx.EnumerateDirectoriesEndsWith(solutionDirectory, "Packages"))
        {

            log($"  Scanning SolutionDir/Packages convention: {solutionPackages}'.");
            AddFiles(ScanDirectoryForPackages(solutionPackages));
        }
    }

    void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            AddFile(file);
        }
    }

    void AddFile(string file)
    {
        if (!string.IsNullOrEmpty(file))
        {
            log($"    Fody weaver file added '{file}'");
            FodyFiles.Add(file);
        }
    }
}