using System;
using Xunit;
using Xunit.Abstractions;

public class ProjectWeaversFinderTests :
    XunitApprovalBase
{
    [Fact]
    public void NotFound()
    {
        var logger = new MockBuildLogger();

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(Environment.CurrentDirectory, Environment.CurrentDirectory, "Release", logger);

        Assert.Empty(configFiles);
    }

    public ProjectWeaversFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}