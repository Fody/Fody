using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaverProjectFileFinderTests
{
    [Test]
    public void Found()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory();
        var combine = Path.Combine(currentDirectory, @"..\..\WeaversProjectFileFinder\WithWeaver");
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));

        var processor = new Processor
            {
                SolutionDir = combine,
                Logger = loggerMock.Object
            };

        processor.FindWeaverProjectFile();
        Assert.IsTrue(processor.FoundWeaverProjectFile);
        loggerMock.Verify();
    }

    [Test]
    public void NotFound()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory();
        var combine = Path.Combine(currentDirectory, @"..\..\WeaversProjectFileFinder\WithNoWeaver");
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));

        var processor = new Processor
            {
                SolutionDir = combine,
                Logger = loggerMock.Object
            };

        processor.FindWeaverProjectFile();
        Assert.IsFalse(processor.FoundWeaverProjectFile);
        loggerMock.Verify();
    }
}