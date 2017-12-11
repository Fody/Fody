using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaverProjectFileFinderTests
{
    [Test]
    public void Found()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var combine = Path.Combine(currentDirectory, "../../../WeaversProjectFileFinder/WithWeaver");
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

        var processor = new Processor
            {
                SolutionDirectory = combine,
                Logger = loggerMock.Object,
        };

        processor.FindWeaverProjectFile();
        Assert.IsTrue(processor.FoundWeaverProjectFile);
        loggerMock.Verify();
    }

    //TODO: add tests where weavers is in references for ncrunch support
    [Test]
    public void NotFound()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var combine = Path.Combine(currentDirectory, "../../../WeaversProjectFileFinder/WithNoWeaver");
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

        var processor = new Processor
            {
                SolutionDirectory = combine,
                Logger = loggerMock.Object,
                References = ""
            };

        processor.FindWeaverProjectFile();
        Assert.IsFalse(processor.FoundWeaverProjectFile);
        loggerMock.Verify();
    }
}