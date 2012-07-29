using System.IO;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class WeaverProjectFileFinderTests
{
    [Test]
    public void Found()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory();
        var combine = Path.Combine(currentDirectory, @"..\..\WeaversProjectFileFinder\WithWeaver");
        var buildLogger = Substitute.For<BuildLogger>();

        var projectFileFinder = new WeaverProjectFileFinder
                                    {
                                        SolutionDir = combine,
                                        Logger = buildLogger
                                    };

        projectFileFinder.Execute();
        buildLogger .Received().LogInfo(Arg.Any<string>());
        Assert.IsTrue(projectFileFinder.Found);
    }
    [Test]
    public void NotFound()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory();
        var combine = Path.Combine(currentDirectory, @"..\..\WeaversProjectFileFinder\WithNoWeaver");
        var buildLogger = Substitute.For<BuildLogger>();

        var projectFileFinder = new WeaverProjectFileFinder
                                    {
                                        SolutionDir = combine,
                                        Logger = buildLogger
                                    };

        projectFileFinder.Execute();
        buildLogger .Received().LogInfo(Arg.Any<string>());
        Assert.IsFalse(projectFileFinder.Found);
    }
}