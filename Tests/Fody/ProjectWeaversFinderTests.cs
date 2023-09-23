public class ProjectWeaversFinderTests
{
    [Fact]
    public void NotFound()
    {
        var logger = new MockBuildLogger();

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(null, Environment.CurrentDirectory, Environment.CurrentDirectory, logger);

        Assert.Empty(configFiles);
    }
}