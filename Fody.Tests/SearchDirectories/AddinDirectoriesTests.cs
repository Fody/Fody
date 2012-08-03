using System.Collections.Generic;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AddinDirectoriesTests
{
    [Test]
    public void Simple()
    {
        var loggerMock = new Mock<BuildLogger>();
        loggerMock.Setup(x => x.LogInfo("Directory added to addin search paths 'Path'."));
        var addinDirectories = new Processor
                                   {
                                       AddinSearchPaths = new List<string> {"Path"},
                                       Logger = loggerMock.Object
                                   };
        addinDirectories.LogAddinSearchPaths();
        loggerMock.Verify();
    }
}