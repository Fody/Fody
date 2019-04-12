using System;
using Moq;
using Xunit;
using Xunit.Abstractions;

public class ProjectWeaversFinderTests :
    XunitLoggingBase
{
    [Fact]
    public void NotFound()
    {
        var logger = new Mock<BuildLogger>(MockBehavior.Loose).Object;

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(Environment.CurrentDirectory, Environment.CurrentDirectory, logger);

        Assert.Empty(configFiles);
    }

    public ProjectWeaversFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}