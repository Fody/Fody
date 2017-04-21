using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaversConfiguredInstanceLinkerTests
{

    [Test]
    public void Should_use_Custom_weaver_from_weaver_project_When_added_to_configured_weavers()
    {
        var mock = new Mock<Processor>();
        mock.Setup(x => x.WeaverProjectContainsType("CustomWeaver"))
            .Returns(true);
        mock.CallBase = true;
        var processor = mock.Object;
        processor.WeaverAssemblyPath = "Path";


        var weaverConfig = new WeaverEntry
        {
            AssemblyName = "CustomWeaver"
        };
        processor.ProcessConfig(weaverConfig);

        Assert.AreEqual("CustomWeaver", weaverConfig.TypeName);
        Assert.AreEqual("Path", weaverConfig.AssemblyPath);
    }

    [Test]
    public void Should_use_named_weaver_When_added_to_configured_weavers()
    {
        var mock = new Mock<Processor>();
        mock.Setup(x => x.WeaverProjectContainsType("AddinName"))
            .Returns(false);
        mock.Setup(x => x.FindAssemblyPath("AddinName")).Returns("Path");

        mock.CallBase = true;

        var processor = mock.Object;

        var weaverConfig = new WeaverEntry
        {
            AssemblyName = "AddinName"
        };
        processor.ProcessConfig(weaverConfig);

        Assert.AreEqual("ModuleWeaver", weaverConfig.TypeName);
        Assert.AreEqual("Path", weaverConfig.AssemblyPath);
        mock.Verify();
    }
}