using System;
using System.IO;
using Fody;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class NugetPackagePathFinderTests
{
    [Test]
    public void GetPackagesPathDefault()
    {
        var weavingTask = new WeavingTask {SolutionDir = Environment.CurrentDirectory};
        var packagePathFinder = new NugetPackagePathFinder
        {
            Logger = Substitute.For<ILogger>(),
            WeavingTask = weavingTask
        };
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine(Environment.CurrentDirectory,"Packages"),packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfig()
    {

        var weavingTask = new WeavingTask { SolutionDir = "DirWithNugetConfig" };
        var packagePathFinder = new NugetPackagePathFinder
        {
            Logger = Substitute.For<ILogger>(),
            WeavingTask = weavingTask
        };
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine("DirWithNugetConfig", "PathFromConfig"), packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfigAndNoPath()
    {
        var weavingTask = new WeavingTask { SolutionDir = "DirWithNugetConfigAndNoPath" };
        var packagePathFinder = new NugetPackagePathFinder
        {
            Logger = Substitute.For<BuildLogger>(),
            WeavingTask = weavingTask
        };
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine("DirWithNugetConfigAndNoPath", "Packages"), packagePathFinder.PackagesPath);
    }
}