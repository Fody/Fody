using System;
using System.IO;
using Moq;
using NUnit.Framework;


[TestFixture]
public class NugetPackagePathFinderTests
{
    [Test]
    public void GetPackagesPathDefault()
    {
        var packagePathFinder = new Processor
                                    {
                                        Logger = new Mock<BuildLogger>().Object,
                                        SolutionDir = Environment.CurrentDirectory
                                    };
        packagePathFinder.FindNugetPackagePath();
        Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "Packages"), packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfig()
    {

        var packagePathFinder = new Processor
                                    {
                                        Logger = new Mock<BuildLogger>().Object,
                                        SolutionDir = "DirWithNugetConfig"
                                    };
        packagePathFinder.FindNugetPackagePath();
        Assert.AreEqual(Path.Combine("DirWithNugetConfig", "PathFromConfig"), packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfigAndNoPath()
    {
        var packagePathFinder = new Processor
                                    {
                                        Logger = new Mock<BuildLogger>().Object,
                                        SolutionDir = "DirWithNugetConfigAndNoPath"
                                    };
        packagePathFinder.FindNugetPackagePath();
        Assert.AreEqual(Path.Combine("DirWithNugetConfigAndNoPath", "Packages"), packagePathFinder.PackagesPath);
    }
}