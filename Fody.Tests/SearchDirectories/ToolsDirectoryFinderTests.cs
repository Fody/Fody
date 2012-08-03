using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ToolsDirectoryFinderTests
{
    [Test]
    public void Simple()
    {
        var logger = new Mock<BuildLogger>().Object;
        var taskTypeLoader = new Processor
                                 {
                                     Logger = logger,
                                     SolutionDir = "Solution"
                                 };
        taskTypeLoader.AddToolsDirectoryToAddinSearch();
        var searchPaths = taskTypeLoader.AddinSearchPaths;
        Assert.AreEqual(Path.GetFullPath(@"Solution\Tools"), searchPaths[0]);
    }
}