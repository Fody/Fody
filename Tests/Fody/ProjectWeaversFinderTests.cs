using System;
using Moq;
using Xunit;

public class ProjectWeaversFinderTests : TestBase
{
    [Fact]
    public void NotFound()
    {
        var logger = new Mock<BuildLogger>(MockBehavior.Loose).Object;

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(Environment.CurrentDirectory, Environment.CurrentDirectory, logger);

        Assert.Empty(configFiles);
    }
}