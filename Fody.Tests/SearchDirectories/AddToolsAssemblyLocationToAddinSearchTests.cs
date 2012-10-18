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
        var expected = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory(), @"..\..\"));
        Assert.AreEqual(expected, processor.AddinSearchPaths[0]+@"\");
    }
}