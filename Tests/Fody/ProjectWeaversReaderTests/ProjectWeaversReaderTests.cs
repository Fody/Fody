using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

public class ProjectWeaversReaderTests : TestBase
{
    [Fact]
    public void Simple()
    {
        var processor = new Processor
                        {
                            ConfigFiles = GetPaths().ToList()
                        };
        processor.ReadProjectWeavers();
        var weavers = processor.Weavers;
        Assert.Equal(3, weavers.Count);
        Assert.Equal("SampleTask1", weavers[0].AssemblyName);
        Assert.Equal("<SampleTask1 MyProperty1=\"PropertyValue2Overwrite\" />", weavers[0].Element);
        Assert.Equal("SampleTask2", weavers[1].AssemblyName);
        Assert.Equal("<SampleTask2 MyProperty2=\"PropertyValue2\" />", weavers[1].Element);
        Assert.Equal("SampleTask3", weavers[2].AssemblyName);
        Assert.Equal("<SampleTask3 MyProperty3=\"PropertyValue3\" />", weavers[2].Element);
    }

    [Fact]
    public void ItLoadsWeaverVersionFilters()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var processor = new Processor
        {
            ConfigFiles = new List<string>
            {
                Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers4.xml")
            }
        };
        processor.ReadProjectWeavers();
        var weavers = processor.Weavers;
        Assert.Equal(3, weavers.Count);
        Assert.Equal("1.2.3", weavers[0].VersionFilter);
        Assert.Equal("(1.2.3, 4.5.*]", weavers[1].VersionFilter);
        Assert.True(string.IsNullOrWhiteSpace(weavers[2].VersionFilter));
    }

    static IEnumerable<string> GetPaths()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers1.xml");
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers2.xml");
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers3.xml");
    }
}