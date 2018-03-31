using System.IO;
#if NET46 // TODO: Remove when ApprovalTests supports .NET Core
using ApprovalTests;
#endif
using Fody;
using Moq;
using Xunit;
// ReSharper disable UnusedVariable

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
#if NET46 // TODO: Remove when ApprovalTests supports .NET Core
        Approvals.Verify(weavingException.Message.Replace(searchDirectory, "SearchDirectory"));
#endif
    }
}