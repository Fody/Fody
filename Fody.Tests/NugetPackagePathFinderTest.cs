using System;
using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
[Ignore]
public class NugetPackagePathFinderTest
{
    [Test]
    public void NoNugetConfig()
    {
        var processor = new Processor
            {
                SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolution")),
                Logger = new Mock<BuildLogger>().Object
            };

        processor.FindNugetPackagePath();
        Assert.IsTrue(processor.PackagesPath.EndsWith("\\FakeSolution\\Packages"));
    }

    [Test]
    public void WithNugetConfig()
    {
        var processor = new Processor
            {
                SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig")),
                Logger = new Mock<BuildLogger>().Object
            };

        processor.FindNugetPackagePath();
        Assert.IsTrue(processor.PackagesPath.EndsWith("\\lib/packages"));
    }

    [Test]
    public void WithNugetConfigInTree()
    {
        var processor = new Processor
            {
                SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfigTreeWalk/SolutionDir")),
                Logger = new Mock<BuildLogger>().Object
            };

        processor.FindNugetPackagePath();
        Assert.IsTrue(processor.PackagesPath.EndsWith("\\lib/packages"));
    }

}