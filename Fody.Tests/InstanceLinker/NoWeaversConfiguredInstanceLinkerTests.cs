using System.Linq;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class NoWeaversConfiguredInstanceLinkerTests
{
    [Test]
    public void Simple()
    {
        var projectWeaversReader = new ProjectWeaversReader();
        var weaverProjectFileFinder = new WeaverProjectFileFinder {Found = true,WeaverAssemblyPath = "Path"};
        var containsWeaverChecker = Substitute.For<WeaverProjectContainsWeaverChecker>();
        containsWeaverChecker.WeaverProjectContainsType("ModuleWeaver").Returns(true);
        var linker = new NoWeaversConfiguredInstanceLinker
                         {
                             ProjectWeaversReader = projectWeaversReader,
                             WeaverProjectFileFinder = weaverProjectFileFinder,
                             Logger = Substitute.For<ILogger>(),
                             WeaverProjectContainsWeaverChecker = containsWeaverChecker
                         };
        linker.Execute();

        var weaverEntry = projectWeaversReader.Weavers.First();
        Assert.AreEqual("ModuleWeaver",weaverEntry.TypeName);
        Assert.AreEqual("Path",weaverEntry.AssemblyPath);
    }
}