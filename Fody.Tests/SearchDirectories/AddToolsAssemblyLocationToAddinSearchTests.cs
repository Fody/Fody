using System.IO;
using NUnit.Framework;

[TestFixture]
public class AddToolsAssemblyLocationToAddinSearchTests
{
    [Test]
    public void Simple()
    {
        var processor = new AddinFinder();
        processor.AddToolsAssemblyLocationToAddinSearch();
        var currentDirectory = AssemblyLocation.CurrentDirectory();
        var expected = Path.GetFullPath(Path.Combine(currentDirectory, @"..\"));
        Assert.AreEqual(expected, processor.AddinSearchPaths[0]+@"\");
    }
}