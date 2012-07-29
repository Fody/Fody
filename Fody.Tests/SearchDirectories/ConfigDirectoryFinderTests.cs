using System.IO;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ConfigDirectoryFinderTests
{
    [Test]
    public void Simple()
    {
        var logger = Substitute.For<ILogger>();
        var searchDirectories = new AddinDirectories
                                    {
                                        Logger = logger
                                    };
        var taskTypeLoader = new ConfigDirectoryFinder
                                 {
                                     AddinDirectories = searchDirectories,
                                     Logger = logger,
                                     AddinSearchPaths = "SearchPath1,SearchPath2",
                                     SolutionDir = "Solution"
                                 };
        taskTypeLoader.Execute();
        var searchPaths = searchDirectories.SearchPaths;
        Assert.AreEqual(Path.GetFullPath(@"Solution\SearchPath1"), searchPaths[0]);
        Assert.AreEqual(Path.GetFullPath(@"Solution\SearchPath2"), searchPaths[1]);
    }

}