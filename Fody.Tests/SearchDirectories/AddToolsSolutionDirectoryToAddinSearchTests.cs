using NUnit.Framework;

[TestFixture]
public class AddToolsSolutionDirectoryToAddinSearchTests
{
    [Test]
    public void Simple()
    {
        var processor = new AddinFinder
            {
                SolutionDirectoryPath = "Solution"
            };
        processor.AddToolsSolutionDirectoryToAddinSearch();
        var searchPaths = processor.AddinSearchPaths;
        Assert.AreEqual(@"Solution\Tools", searchPaths[0]);
    }
}