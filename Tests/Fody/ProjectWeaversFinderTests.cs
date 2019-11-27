using System;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ProjectWeaversFinderTests :
    VerifyBase
{
    [Fact]
    public void NotFound()
    {
        var logger = new MockBuildLogger();

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(Environment.CurrentDirectory, Environment.CurrentDirectory, logger);

        Assert.Empty(configFiles);
    }

    public ProjectWeaversFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}