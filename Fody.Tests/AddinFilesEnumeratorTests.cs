using System.IO;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class AddinFilesEnumeratorTests
{
    [Test]
    public void NotFound()
    {
        var searchDirectories = Substitute.For<AddinDirectories>();
        searchDirectories.SearchPaths.Add(Path.GetFullPath("Packages"));
        var taskTypeLoader = new AddinFilesEnumerator { AddinDirectories = searchDirectories };
        Assert.IsNull(taskTypeLoader.FindAddinAssembly("DoesNotExist"));
    }

    [Test]
    public void Valid()
    {
        var searchDirectories = Substitute.For<AddinDirectories>();
        searchDirectories.SearchPaths.Add(Path.GetFullPath("Packages"));
        var taskTypeLoader = new AddinFilesEnumerator { AddinDirectories = searchDirectories };
        taskTypeLoader.FindAddinAssembly("SampleTask.Fody");
    }
}