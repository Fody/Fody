using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
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
        var configFiles = ConfigFileFinder.FindWeaverConfigs(Environment.CurrentDirectory, Environment.CurrentDirectory, logger);
        Assert.IsEmpty(configFiles);
        loggerMock.Verify();
    }

    [Test]
    public void Test_FindWeaverConfigs_WhenConfigurationFileExist_ShouldFindFile()
    {
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

        MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { AssemblyLocation.CurrentDirectory + @"\FodyWeavers.xml", new MockFileData("<Weavers></Weavers>")}
        });

        BuildLogger logger = loggerMock.Object;
        var configFiles = ConfigFileFinder.FindWeaverConfigs(Environment.CurrentDirectory, Environment.CurrentDirectory, logger, mockFileSystem);

        Assert.IsNotEmpty(configFiles);
        loggerMock.Verify();
    }

    [Test]
    public void Test_FindWeaverConfigs_WhenFileNotFound_ShouldLogError()
    {
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

        BuildLogger logger = loggerMock.Object;
        ConfigFileFinder.FindWeaverConfigs(Environment.CurrentDirectory, Environment.CurrentDirectory, logger);

        loggerMock.Verify(x => x.LogDebug(It.Is<string>(v => v.Contains("Could not find path to weavers file"))), Times.AtLeastOnce());
    }
}