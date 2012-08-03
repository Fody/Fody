using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ConfigDirectoryFinderTests
{
    [Test]
    public void Simple()
    {
        var logger = new Mock<BuildLogger>().Object;
        var taskTypeLoader = new Processor
                                 {
                                     Logger = logger,
                                     AddinSearchPathsFromMsBuild = "SearchPath1,SearchPath2",
                                     SolutionDir = "Solution"
                                 };
        taskTypeLoader.AddMsBuildConfigToAddinSearch();
        var searchPaths = taskTypeLoader.AddinSearchPaths;
        Assert.AreEqual(Path.GetFullPath(@"Solution\SearchPath1"), searchPaths[0]);
        Assert.AreEqual(Path.GetFullPath(@"Solution\SearchPath2"), searchPaths[1]);
    }

}