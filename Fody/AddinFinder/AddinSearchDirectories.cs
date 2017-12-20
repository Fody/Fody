using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    public void FindAddinDirectories()
    {
        Logger.LogDebug("FindAddinDirectories:");
        if (PackageDefinitions == null)
        {
            Logger.LogDebug("  No PackageDefinitions");

            AddNugetDirectoryFromConvention();
            AddNugetDirectoryFromNugetConfig();
            AddDerivePackagesFromMsBuildThisFileDirectory();
            AddToolsSolutionDirectoryToAddinSearch();
            AddNuGetPackageRootToAddinSearch();
        }
        else
        {
            var separator = $"{Environment.NewLine}    - ";
            var packageDefinitionsLogMessage = separator + string.Join(separator, PackageDefinitions);
            Logger.LogDebug($"  PackageDefinitions: {packageDefinitionsLogMessage}");

            // each PackageDefinition will be of the format C:\...\packages\propertychanging.fody\1.28.0
            // so must be a Contains(.fody)
            foreach (var versionDirectory in PackageDefinitions.Where(x => x.ToLowerInvariant().Contains(".fody")))
            {
                Logger.LogDebug($"  Scannin package directory: '{versionDirectory}'");

                var netClassic = Path.Combine(versionDirectory, "netclassicweaver");
                if (Directory.Exists(netClassic))
                {
                    AddFiles(Directory.EnumerateFiles(netClassic, "*.Fody.dll"));
                    continue;
                }

                AddFiles(Directory.EnumerateFiles(versionDirectory, "*.Fody.dll"));
            }
            AddToolsSolutionDirectoryToAddinSearch();
        }
    }

    void AddNuGetPackageRootToAddinSearch()
    {
        if (NuGetPackageRoot == null)
        {
            Logger.LogDebug($"  Skipped NuGetPackageRoot since it is not defined.");
            return;
        }
        if (!Directory.Exists(NuGetPackageRoot))
        {
            Logger.LogDebug($"  Skipped NuGetPackageRoot '{NuGetPackageRoot}' since it doesn't exist.");
        }
        Logger.LogDebug($"  Scanning NuGetPackageRoot '{NuGetPackageRoot}'.");
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
                var netClassic = Path.Combine(versionDirectory, @"netclassicweaver\", lowercasePackageName);
                if (File.Exists(netClassic))
                {
                    yield return netClassic;
                    yield break;
                }

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
        var solutionDirToolsDirectory = Path.Combine(SolutionDirectoryPath, "Tools");

        if (!Directory.Exists(solutionDirToolsDirectory))
        {
            Logger.LogDebug($"  Skipped scanning '{solutionDirToolsDirectory}' since it doesn't exist.");
            return;
        }

        Logger.LogDebug($"  Scanning SolutionDir/Tools directory convention: '{solutionDirToolsDirectory}'.");
        AddFiles(Directory.EnumerateFiles(solutionDirToolsDirectory, "*.Fody.dll", SearchOption.AllDirectories));
    }

    public void AddDerivePackagesFromMsBuildThisFileDirectory()
    {
        var fromMsBuildThisFileDirectory = Path.Combine(MsBuildThisFileDirectory,@"..\..\..\");
        Logger.LogDebug($"  Scanning the MsBuildThisFileDirectory parent: {fromMsBuildThisFileDirectory}'.");
        AddWeaversFromDir(fromMsBuildThisFileDirectory);
    }

    public void AddNugetDirectoryFromNugetConfig()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(SolutionDirectoryPath);
        if (packagesPathFromConfig == null)
        {
            Logger.LogDebug("  Skipped directory from Nuget Config since it could not be derived.");
            return;
        }
        if (!Directory.Exists(packagesPathFromConfig))
        {
            Logger.LogDebug($"  Skipped directory from Nuget Config '{packagesPathFromConfig}' since it doesn't exist.");
            return;
        }
        Logger.LogDebug($"  Scanning directory from Nuget Config: {packagesPathFromConfig}'.");
        AddWeaversFromDir(packagesPathFromConfig);
    }

    public void AddNugetDirectoryFromConvention()
    {
        var solutionPackages = Path.Combine(SolutionDirectoryPath, "Packages");
        if (!Directory.Exists(solutionPackages))
        {
            Logger.LogDebug($"  Skipped SolutionDir/Packages convention '{solutionPackages}' since it doesn't exist.");
            return;
        }
        Logger.LogDebug($"  Scanning SolutionDir/Packages convention: {solutionPackages}'.");
        AddWeaversFromDir(solutionPackages);
    }

    void AddWeaversFromDir(string directory)
    {
        Logger.LogDebug($"    Adding weaver dlls from '{directory}'.");
        foreach (var packageDir in Directory.GetDirectories(directory, "*.Fody*"))
        {
            AddFiles(Directory.EnumerateFiles(packageDir, "*.Fody.dll"));
        }
    }

    void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            Logger.LogDebug($"    Fody weaver file added '{file}'");
            FodyFiles.Add(file);
        }
    }

    public ILogger Logger;
    public string SolutionDirectoryPath;
    public string MsBuildThisFileDirectory;
    public string NuGetPackageRoot;
    public List<string> PackageDefinitions;
}