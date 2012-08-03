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
        var containsWeaverCheckerMock = new Mock<Processor>();
        containsWeaverCheckerMock
            .Setup(x => x.WeaverProjectContainsType("ModuleWeaver"))
            .Returns(true);
        var innerWeavingTask = containsWeaverCheckerMock.Object;

        innerWeavingTask.WeaverAssemblyPath = "Path";
        innerWeavingTask.FoundWeaverProjectFile = true;
        innerWeavingTask.Weavers = new List<WeaverEntry>();
        innerWeavingTask.Logger = new Mock<BuildLogger>().Object;

        innerWeavingTask.ConfigureWhenNoWeaversFound();

        var weaverEntry = innerWeavingTask.Weavers.First();
        Assert.AreEqual("ModuleWeaver",weaverEntry.TypeName);
        Assert.AreEqual("Path",weaverEntry.AssemblyPath);
        containsWeaverCheckerMock.Verify();
    }
}