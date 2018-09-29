using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public partial class AddinFinder
{
    private const string WeaverDllSuffix = ".Fody.dll";

    readonly Action<string> log;
    readonly string solutionDirectory;
    readonly string msBuildTaskDirectory;
    readonly string nuGetPackageRoot;
    readonly string weaverProbingPaths;

    IDictionary<string, string> weaversFromWellKnownPaths;

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
        var weaverFiles = EnumerateAddinsFromProbingPaths(weaverProbingPaths)
            .Concat(EnumerateToolsSolutionDirectoryWeavers())
            .Concat(EnumerateInSolutionWeavers());

        weaversFromWellKnownPaths = BuildWeaversDictionary(weaverFiles);
    }

    public static IEnumerable<string> EnumerateAddinsFromProbingPaths(string weaverProbingPaths)
    {
        var probingPaths = weaverProbingPaths?.Split(';')
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => item.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            .Select(Path.GetDirectoryName) // .props file is in the build sub-directory => package root is the parent folder.
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => item.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
            .OrderByDescending(item => item.Length)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var waverFiles = probingPaths?
            .Select(path => GetAssemblyFromNugetDir(path, null))
            .Where(path => !string.IsNullOrEmpty(path))
            .ToArray();

        return waverFiles ?? Enumerable.Empty<string>();
    }

    public static Dictionary<string, string> BuildWeaversDictionary(IEnumerable<string> waverFiles)
    {
        return waverFiles?
               .Where(item => item != null)
               .ToDictionary(GetAddinNameFromWeaverFile, StringComparer.OrdinalIgnoreCase) 
           ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    static string GetAddinNameFromWeaverFile(string filePath)
    {
        Debug.Assert(filePath.EndsWith(WeaverDllSuffix, StringComparison.OrdinalIgnoreCase));

        // remove .Fody.dll
        return Path.ChangeExtension(Path.GetFileNameWithoutExtension(filePath), null);
    }

    IEnumerable<string> FindAddinDirectoriesLegacy()
    {
        log("FindAddinDirectories (Legacy):");

        return EnumerateNugetDirectoryFromNugetConfig()
            .Concat(EnumerateWeaversFromMsBuildDirectory())
            .Concat(EnumerateNuGetPackageRoot())
            .Where(item => item != null);
    }

    IEnumerable<string> EnumerateNuGetPackageRoot()
    {
        if (nuGetPackageRoot == null)
        {
            log("  Skipped NuGetPackageRoot since it is not defined.");
            return Enumerable.Empty<string>();
        }
        if (!Directory.Exists(nuGetPackageRoot))
        {
            log($"  Skipped NuGetPackageRoot '{nuGetPackageRoot}' since it doesn't exist.");
            return Enumerable.Empty<string>();
        }
        log($"  Scanning NuGetPackageRoot '{nuGetPackageRoot}'.");

        return ScanDirectoryForPackages(nuGetPackageRoot);
    }

    public static IEnumerable<string> ScanDirectoryForPackages(string directory)
    {
        return EnumerateOldStyleDirectories(directory)
            .Concat(EnumerateNewOrPaketStyleDirectories(directory)
            .Where(x => x != null));
    }

    static IEnumerable<string> EnumerateNewOrPaketStyleDirectories(string directory)
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

    static IEnumerable<string> EnumerateOldStyleDirectories(string directory)
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

        if (packageName == null)
        {
            return Directory.GetFiles(dir)
                .Select(x => new FileInfo(x))
                .FirstOrDefault(x => x.Name.EndsWith(WeaverDllSuffix, StringComparison.OrdinalIgnoreCase))?.FullName;
        }

        return Directory.GetFiles(dir)
            .Select(x => new FileInfo(x))
            .FirstOrDefault(x => x.Name.Equals($"{packageName}.dll", StringComparison.OrdinalIgnoreCase))?.FullName;
    }

    IEnumerable<string> EnumerateToolsSolutionDirectoryWeavers()
    {
        var solutionDirToolsDirectory = Path.Combine(solutionDirectory, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            log($"  Skipped scanning '{solutionDirToolsDirectory}' since it doesn't exist.");
            return Enumerable.Empty<string>();
        }

        log($"  Scanning SolutionDir/Tools directory convention: '{solutionDirToolsDirectory}'.");
        
        return DirectoryEx.EnumerateFilesEndsWith(solutionDirToolsDirectory, WeaverDllSuffix, SearchOption.AllDirectories);
    }

    IEnumerable<string> EnumerateWeaversFromMsBuildDirectory()
    {
        var fromMsBuildThisFileDirectory = Path.GetFullPath(Path.Combine(msBuildTaskDirectory, "../../../"));
        if (!Directory.Exists(fromMsBuildThisFileDirectory))
        {
            log($"  Skipped scanning '{fromMsBuildThisFileDirectory}' since it doesn't exist.");
            return Enumerable.Empty<string>();
        }

        log($"  Scanning the MsBuildThisFileDirectory parent: {fromMsBuildThisFileDirectory}'.");
        return ScanDirectoryForPackages(fromMsBuildThisFileDirectory);
    }

    IEnumerable<string> EnumerateNugetDirectoryFromNugetConfig()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDirectory);
        if (packagesPathFromConfig == null)
        {
            log("  Skipped directory from Nuget Config since it could not be derived.");
            return Enumerable.Empty<string>();
        }
        if (!Directory.Exists(packagesPathFromConfig))
        {
            log($"  Skipped directory from Nuget Config '{packagesPathFromConfig}' since it doesn't exist.");
            return Enumerable.Empty<string>();
        }
        log($"  Scanning directory from Nuget Config: {packagesPathFromConfig}'.");
        
        return ScanDirectoryForPackages(packagesPathFromConfig);
    }

    IEnumerable<string> EnumerateInSolutionWeavers()
    {
        IEnumerable<string> ScanDirectory(string directory)
        {
            log($"  Scanning SolutionDir/Packages convention: {directory}'.");
            return ScanDirectoryForPackages(directory);
        }

        return DirectoryEx.EnumerateDirectoriesEndsWith(solutionDirectory, "Packages")
            .SelectMany(ScanDirectory);
    }
}