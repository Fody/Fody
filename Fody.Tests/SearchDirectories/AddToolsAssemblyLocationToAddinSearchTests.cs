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
        Assert.AreEqual(Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory(), @"..\..\")), processor.AddinSearchPaths[0]+@"\");
    }
}