using System.IO;
using Fody;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ToolsDirectoryFinderTests
{
    [Test]
    public void Simple()
    {
        var weavingTask = new WeavingTask {SolutionDir = "Solution"};
        var logger = Substitute.For<BuildLogger>();
        var searchDirectories = new AddinDirectories
                                    {
                                        Logger = logger
                                    };
        var taskTypeLoader = new ToolsDirectoryFinder
                                 {
                                     AddinDirectories = searchDirectories,
                                     Logger = logger,
                                     WeavingTask = weavingTask
                                 };
        taskTypeLoader.Execute();
        var searchPaths = searchDirectories.SearchPaths;
        Assert.AreEqual(Path.GetFullPath(@"Solution\Tools"), searchPaths[0]);
    }
}