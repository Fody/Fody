using System.IO;
using Moq;
using Xunit;

public class WeaverProjectFileFinderTests : TestBase
{
    [Fact]
    public void Found()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var combine = Path.Combine(currentDirectory, @"..\..\..\Fody\WeaversProjectFileFinder\WithWeaver");
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

        var processor = new Processor
        {
            SolutionDirectory = combine,
            Logger = loggerMock.Object,
            References = ""
        };

        processor.FindWeaverProjectFile();
        Assert.True(processor.FoundWeaverProjectFile);
        loggerMock.Verify();
    }

    //TODO: add tests where weavers is in references for ncrunch support
    [Fact]
    public void NotFound()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var combine = Path.Combine(currentDirectory, "../../../Fody/WeaversProjectFileFinder/WithNoWeaver");
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

        var processor = new Processor
            {
                SolutionDirectory = combine,
                Logger = loggerMock.Object,
                References = ""
            };

        processor.FindWeaverProjectFile();
        Assert.False(processor.FoundWeaverProjectFile);
        loggerMock.Verify();
    }
}