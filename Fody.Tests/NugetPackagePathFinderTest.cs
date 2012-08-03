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
        var runner = new Processor
                         {
                             SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolution")),
                             Logger = new Mock<BuildLogger>().Object
                         };

        runner.FindNugetPackagePath();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\FakeSolution\\Packages"));
    }

    [Test]
    public void WithNugetConfig()
    {
        var runner = new Processor
                         {
                             SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig")),
                             Logger = new Mock<BuildLogger>().Object
                         };

        runner.FindNugetPackagePath();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\lib/packages"));
    }
    [Test]
    public void WithNugetConfigInTree()
    {
        var runner = new Processor
                         {
                             SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfigTreeWalk/SolutionDir")),
                             Logger = new Mock<BuildLogger>().Object
                         };

        runner.FindNugetPackagePath();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\lib/packages"));
    }

}