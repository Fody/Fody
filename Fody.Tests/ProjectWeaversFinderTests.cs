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
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));
        var logger = loggerMock.Object;
        var configFiles = ConfigFileFinder.FindProjectWeavers(Environment.CurrentDirectory, Environment.CurrentDirectory, logger);
        Assert.IsEmpty(configFiles);
        loggerMock.Verify();
        
    }
}