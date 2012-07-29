using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
[Ignore]
public class NugetPackagePathFinderTest
{
    [Test]
    public void NoNugetConfig()
    {
        var runner = new NugetPackagePathFinder
                         {
                             SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolution")),
                             Logger = Substitute.For<BuildLogger>()
                         };

        runner.Execute();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\FakeSolution\\Packages"));
    }

    [Test]
    public void WithNugetConfig()
    {
        var runner = new NugetPackagePathFinder
                         {
                             SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig")),
                             Logger = Substitute.For<BuildLogger>()
                         };

        runner.Execute();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\lib/packages"));
    }
    [Test]
    public void WithNugetConfigInTree()
    {
        var runner = new NugetPackagePathFinder
                         {
                             SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfigTreeWalk/SolutionDir")),
                             Logger = Substitute.For<BuildLogger>()
                         };

        runner.Execute();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\lib/packages"));
    }

}