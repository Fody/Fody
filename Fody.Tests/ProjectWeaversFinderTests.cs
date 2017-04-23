using System;
using ApprovalTests;
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
        var weavingException = Assert.Throws<WeavingException>(() => ConfigFileFinder.FindWeaverConfigs(Environment.CurrentDirectory, Environment.CurrentDirectory, logger));
        Approvals.Verify(weavingException.Message);
        
    }
}