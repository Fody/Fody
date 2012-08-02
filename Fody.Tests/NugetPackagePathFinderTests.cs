using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class NugetPackagePathFinderTests
{
    [Test]
    public void GetPackagesPathDefault()
    {
        var packagePathFinder = new NugetPackagePathFinder
        {
            Logger = Substitute.For<ILogger>(),
            SolutionDir = Environment.CurrentDirectory
        };
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine(Environment.CurrentDirectory,"Packages"),packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfig()
    {

        var packagePathFinder = new NugetPackagePathFinder
        {
            Logger = Substitute.For<ILogger>(),
            SolutionDir = "DirWithNugetConfig" 
        };
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine("DirWithNugetConfig", "PathFromConfig"), packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfigAndNoPath()
    {
        var packagePathFinder = new NugetPackagePathFinder
        {
            Logger = Substitute.For<ILogger>(),
            SolutionDir = "DirWithNugetConfigAndNoPath" 
        };
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine("DirWithNugetConfigAndNoPath", "Packages"), packagePathFinder.PackagesPath);
    }
}