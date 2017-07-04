using System.IO;
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
        var testDirectory = TestContext.CurrentContext.TestDirectory;
        var searchDirectory = Path.Combine(testDirectory, "FodyWeavers.xml");

        var weavingException = Assert.Throws<WeavingException>(() => ConfigFileFinder.FindWeaverConfigs(testDirectory, testDirectory, logger));
        Assert.AreEqual(weavingException.Message, $"Could not find path to weavers file. Searched '{searchDirectory}', '{searchDirectory}'.");
    }
}