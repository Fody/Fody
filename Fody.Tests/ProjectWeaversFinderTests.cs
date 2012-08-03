using System;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ProjectWeaversFinderTests
{
    [Test]
    public void NotFound()
    {
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));
        var logger = loggerMock.Object;
        var projectWeaversFinder = new Processor
                                       {
                                           ProjectPath = Environment.CurrentDirectory,
                                           Logger = logger,
                                           SolutionDir = Environment.CurrentDirectory
                                       };
        projectWeaversFinder.FindProjectWeavers();
        Assert.IsEmpty(projectWeaversFinder.ConfigFiles);
        loggerMock.Verify();
        
    }
}