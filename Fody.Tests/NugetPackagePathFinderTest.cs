using System;
using System.IO;
using Fody;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
[Ignore]
public class NugetPackagePathFinderTest
{
    [Test]
    public void NoNugetConfig()
    {
        var weavingTask = new WeavingTask
                                      {
                                          SolutionDir = Path.GetFullPath( Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolution"))
                                      };
        var runner = new NugetPackagePathFinder
                         {
                             WeavingTask = weavingTask,
                             Logger = Substitute.For<BuildLogger>()
                         };

        runner.Execute();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\FakeSolution\\Packages"));
    }

    [Test]
    public void WithNugetConfig()
    {
        var weavingTask = new WeavingTask
                                      {
                                          SolutionDir =Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig"))
                                      };
        var runner = new NugetPackagePathFinder
                         {
                             WeavingTask = weavingTask,
                             Logger = Substitute.For<BuildLogger>()
                         };

        runner.Execute();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\lib/packages"));
    }
    [Test]
    public void WithNugetConfigInTree()
    {
        var weavingTask = new WeavingTask
                                      {
                                          SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfigTreeWalk/SolutionDir"))
                                      };
        var runner = new NugetPackagePathFinder
                         {
                             WeavingTask = weavingTask,
                             Logger = Substitute.For<BuildLogger>()
                         };

        runner.Execute();
        Assert.IsTrue(runner.PackagesPath.EndsWith("\\lib/packages"));
    }

}