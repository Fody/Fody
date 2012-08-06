using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class NugetConfigReaderTest
{
    [Test]
    public void WithNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("\\lib/packages"));
    }

    [Test]
    public void WithNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig/Foo"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);

        Assert.IsTrue(packagesPathFromConfig.EndsWith("\\lib/packages"));
    }

}