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
        var processor = new AddinFinder
            {
                Logger = new Mock<BuildLogger>().Object,
                SolutionDirectoryPath = Environment.CurrentDirectory
            };
        processor.FindNugetPackagePath();
        Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "Packages"), processor.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfig()
    {
        var processor = new AddinFinder
            {
                Logger = new Mock<BuildLogger>().Object,
                SolutionDirectoryPath = "DirWithNugetConfig"
            };
        processor.FindNugetPackagePath();
        Assert.AreEqual(Path.Combine("DirWithNugetConfig", "PathFromConfig"), processor.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfigAndNoPath()
    {
        var processor = new AddinFinder
            {
                Logger = new Mock<BuildLogger>().Object,
                SolutionDirectoryPath = "DirWithNugetConfigAndNoPath"
            };
        processor.FindNugetPackagePath();
        Assert.AreEqual(Path.Combine("DirWithNugetConfigAndNoPath", "Packages"), processor.PackagesPath);
    }
}