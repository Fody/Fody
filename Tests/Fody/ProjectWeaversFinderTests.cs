public class ProjectWeaversFinderTests
{
    [Test]
    public async Task NotFound()
    {
        var logger = new MockBuildLogger();

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(null, Environment.CurrentDirectory, Environment.CurrentDirectory, logger);

        await Assert.That(configFiles).IsEmpty();
    }
}