using System.IO;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class PackagePathFinderTests
{
    [Test]
    public void GetPackagesPathDefault()
    {
        var solutionPathFinder = Substitute.For<SolutionPathFinder>(null, null, null);
        solutionPathFinder.SolutionDirectory = "";
        var packagePathFinder = new PackagePathFinder(Substitute.For<Logger>(), solutionPathFinder);
        packagePathFinder.Execute();
        Assert.AreEqual("Packages",packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfig()
    {
        var solutionPathFinder = Substitute.For<SolutionPathFinder>(null,null,null);
        solutionPathFinder.SolutionDirectory = "DirWithNugetConfig";
        var packagePathFinder = new PackagePathFinder(Substitute.For<Logger>(), solutionPathFinder);
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine("DirWithNugetConfig", "PathFromConfig"), packagePathFinder.PackagesPath);
    }

    [Test]
    public void GetPackagesPathWithNugetConfigAndNoPath()
    {
        var solutionPathFinder = Substitute.For<SolutionPathFinder>(null, null, null);
        solutionPathFinder.SolutionDirectory = "DirWithNugetConfigAndNoPath";
        var packagePathFinder = new PackagePathFinder(Substitute.For<Logger>(), solutionPathFinder);
        packagePathFinder.Execute();
        Assert.AreEqual(Path.Combine("DirWithNugetConfigAndNoPath", "Packages"), packagePathFinder.PackagesPath);
    }
}