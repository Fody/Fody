using System;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ProjectWeaversFinderTests
{
    [Test]
    public void NotFound()
    {
        var logger = Substitute.For<ILogger>();
        var projectWeaversFinder = new ProjectWeaversFinder
                                       {
                                           ProjectFilePath = Environment.CurrentDirectory,
                                           Logger = logger,
                                           SolutionDir = Environment.CurrentDirectory
                                       };
        projectWeaversFinder.Execute();
        Assert.IsEmpty(projectWeaversFinder.ConfigFiles);
        logger.Received(1).LogInfo(Arg.Any<string>());
    }
}