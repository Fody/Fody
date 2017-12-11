using Moq;
using Xunit;

public class WeaversConfiguredInstanceLinkerTests : TestBase
{
    [Fact]
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

        Assert.Equal("CustomWeaver", weaverConfig.TypeName);
        Assert.Equal("Path", weaverConfig.AssemblyPath);
    }

    [Fact]
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

        Assert.Equal("ModuleWeaver", weaverConfig.TypeName);
        Assert.Equal("Path", weaverConfig.AssemblyPath);
        mock.Verify();
    }
}