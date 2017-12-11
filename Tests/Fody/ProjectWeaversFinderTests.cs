using System.IO;
using ApprovalTests;
using Moq;
using Xunit;

public class ProjectWeaversFinderTests : TestBase
{
    [Fact]
    public void NotFound()
    {
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));
        var logger = loggerMock.Object;
        var searchDirectory = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyWeavers.xml");

        var weavingException = Assert.Throws<WeavingException>(
            () => ConfigFileFinder.FindWeaverConfigs(AssemblyLocation.CurrentDirectory, AssemblyLocation.CurrentDirectory, logger));
        Approvals.Verify(weavingException.Message.Replace(searchDirectory, "SearchDirectory"));
    }
}