using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AddToolsAssemblyLocationToAddinSearchTests
{
    [Test]
    public void Simple()
    {
        var path = Path.Combine(Directory.GetParent(AssemblyLocation.CurrentDirectory()).FullName, "Tools");
        Directory.CreateDirectory(path);
        var logger = new Mock<BuildLogger>().Object;
        var processor = new Processor
            {
                Logger = logger,
                SolutionDir = "Solution"
            };
        processor.AddToolsAssemblyLocationToAddinSearch();
        var searchPaths = processor.AddinSearchPaths;
        Assert.AreEqual(path, searchPaths[0]);
    }
}