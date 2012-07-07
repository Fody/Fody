using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class WeaversConfiguredInstanceLinkerTests
{

    [Test]
    public void CustomWeaverInWeaversProject()
    {
        var weaverProjectFileFinder = new WeaverProjectFileFinder
                                          {
                                              WeaverAssemblyPath = "Path",
                                              
                                              
                                          };
        var containsWeaverChecker = Substitute.For<WeaverProjectContainsWeaverChecker>();
        containsWeaverChecker.WeaverProjectContainsType("CustomWeaver").Returns(true);
        var linker = new WeaversConfiguredInstanceLinker
                         {
                             WeaverProjectFileFinder = weaverProjectFileFinder,
                             WeaverProjectContainsWeaverChecker = containsWeaverChecker,
                             
                         };
        var weaverConfig = new WeaverEntry
                               {
                                   AssemblyName = "CustomWeaver"
                               };
        linker.ProcessConfig(weaverConfig);

        Assert.AreEqual("CustomWeaver", weaverConfig.TypeName);
        Assert.AreEqual("Path",weaverConfig.AssemblyPath);
    }

    [Test]
    public void WeaverInAddin()
    {
        var containsWeaverChecker = Substitute.For<WeaverProjectContainsWeaverChecker>();
        containsWeaverChecker.WeaverProjectContainsType("AddinName")
            .Returns(false);

        var weaverAssemblyPathFinder = Substitute.For<WeaverAssemblyPathFinder>();
        weaverAssemblyPathFinder .FindAssemblyPath("AddinName")
            .Returns("Path");

        var linker = new WeaversConfiguredInstanceLinker
                         {
                             WeaverAssemblyPathFinder = weaverAssemblyPathFinder,
                             WeaverProjectContainsWeaverChecker = containsWeaverChecker,
                             
                         };
        var weaverConfig = new WeaverEntry
                               {
                                   AssemblyName = "AddinName"
                               };
        linker.ProcessConfig(weaverConfig);

        Assert.AreEqual("ModuleWeaver", weaverConfig.TypeName);
        Assert.AreEqual("Path",weaverConfig.AssemblyPath);
    }
}