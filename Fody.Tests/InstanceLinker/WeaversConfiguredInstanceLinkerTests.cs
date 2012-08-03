using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaversConfiguredInstanceLinkerTests
{

    [Test]
    public void CustomWeaverInWeaversProject()
    {
        var mock = new Mock<Processor>();
        mock.Setup(x => x.WeaverProjectContainsType("CustomWeaver"))
            .Returns(true);
        var processor = mock.Object;
        processor.WeaverAssemblyPath = "Path";


        var weaverConfig = new WeaverEntry
                               {
                                   AssemblyName = "CustomWeaver"
                               };
        processor.ProcessConfig(weaverConfig);

        Assert.AreEqual("CustomWeaver", weaverConfig.TypeName);
        Assert.AreEqual("Path",weaverConfig.AssemblyPath);
    }

    [Test]
    public void WeaverInAddin()
    {


        var mock = new Mock<Processor>();
        mock.Setup(x => x.WeaverProjectContainsType("AddinName"))
            .Returns(false);
        mock.Setup(x => x.FindAssemblyPath("AddinName")).Returns("Path");

        var processor = mock.Object;
        
        var weaverConfig = new WeaverEntry
                               {
                                   AssemblyName = "AddinName"
                               };
        processor.ProcessConfig(weaverConfig);

        Assert.AreEqual("ModuleWeaver", weaverConfig.TypeName);
        Assert.AreEqual("Path",weaverConfig.AssemblyPath);
        mock.Verify();
    }
}