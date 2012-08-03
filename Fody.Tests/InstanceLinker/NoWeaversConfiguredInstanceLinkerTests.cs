using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

[TestFixture]
public class NoWeaversConfiguredInstanceLinkerTests
{
    [Test]
    public void Simple()
    {
        var mock = new Mock<Processor>();
        mock.Setup(x => x.WeaverProjectContainsType("ModuleWeaver"))
            .Returns(true);
        var processor = mock.Object;

        processor.WeaverAssemblyPath = "Path";
        processor.FoundWeaverProjectFile = true;
        processor.Weavers = new List<WeaverEntry>();
        processor.Logger = new Mock<BuildLogger>().Object;

        processor.ConfigureWhenNoWeaversFound();

        var weaverEntry = processor.Weavers.First();
        Assert.AreEqual("ModuleWeaver",weaverEntry.TypeName);
        Assert.AreEqual("Path",weaverEntry.AssemblyPath);
        mock.Verify();
    }
}