using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class AddinDirectoriesTests
{
    [Test]
    public void Simple()
    {
        var buildLogger = Substitute.For<ILogger>();
        var addinDirectories = new AddinDirectories
                                   {
                                       SearchPaths = new List<string> {"Path"},
                                       Logger = buildLogger
                                   };
        addinDirectories.Execute();
        buildLogger.Received().LogInfo("Directory added to addin search paths 'Path'.");
    }
}