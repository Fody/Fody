using System;
using System.Collections.Generic;

using Moq;
using Xunit;

public class WeaverAssemblyPathFinderTests : TestBase
{
    [Fact(Skip = "todo")]
    //TODO:
    public void Valid()
    {
        var finder = new Processor
        {
            ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object,
        };
        finder.FindAssemblyPath("Name");
    }

    const string PackageName = "Package1";
    const string PackageWeaverName = PackageName + ".Fody";
    const string PackageFileName = PackageWeaverName + ".dll";

    const string NewPackageFolder = @"c:\temp\packages\Package1\1.1";
    const string OldPackageFolder = @"c:\temp\packages\Package1\1.0";
    const string VeryOldPackageFolder = @"c:\anotherFolder\packages\Package1\0.8";
    const string SomeOtherPackageFolders = @";c:\temp\packages\Package2\1.1\build\;;c:\temp\packages\Package3\1.1\build;";

    const string NewPackagePath = NewPackageFolder + "\\" + PackageFileName;
    const string OldPackagePath = OldPackageFolder + "\\" + PackageFileName;
    const string VeryOldPackagePath = VeryOldPackageFolder + "\\" + PackageFileName;

    [Theory]
    [InlineData(null, NewPackagePath)] // find the latest package without probing path
    [InlineData(@";" + NewPackageFolder + @"\build\", NewPackagePath)] // find the new package with probing path
    [InlineData(@";" + OldPackageFolder + @"\build\", OldPackagePath)] // find the old package with probing path
    [InlineData(@";" + VeryOldPackageFolder + @"\build\", VeryOldPackagePath)] // find the old package with probing path
    [InlineData(@";c:\temp\packages\Package42\1.1\build\", NewPackagePath)] // find the latest package with probing paths only from different packages
    public void FindPathsWithProbingTest(string probingPaths, string expected, string packageName = "Package1")
    {
        if (probingPaths != null)
            probingPaths = SomeOtherPackageFolders + probingPaths + SomeOtherPackageFolders;

        var finder = new AddinFinder(_ => { }, null, null, null, null, probingPaths);

        var assemblyVersions = new Dictionary<string, Version>(StringComparer.OrdinalIgnoreCase)
        {
            { VeryOldPackagePath, new Version(0, 8) },
            { OldPackagePath, new Version(1, 0) },
            { NewPackagePath, new Version(1, 1) },
        };

        finder.FodyFiles.AddRange(assemblyVersions.Keys);

        finder.VersionReader = path => assemblyVersions[path];

        var target = finder.FindAddinAssembly(packageName);

        Assert.Equal(expected, target);
    }
}