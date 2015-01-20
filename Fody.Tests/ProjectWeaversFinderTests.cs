using System;
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
        var processor = new Processor
                                       {
                                           ProjectDirectory = Environment.CurrentDirectory,
                                           Logger = logger,
                                           SolutionDirectoryPath = Environment.CurrentDirectory
                                       };

        var configuration = new Configuration(processor.Logger, processor.SolutionDirectoryPath,
            processor.ProjectDirectory, processor.DefineConstants);

        Assert.IsEmpty(configuration.ConfigFiles);
        loggerMock.Verify();
    }
}