using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalTests;
using Fody;
using Xunit;

public class ProjectWeaversReaderTests : TestBase
{
    [Fact]
    public void Invalid()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var path = Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\Invalid.txt");

        var exception = Assert.Throws<WeavingException>(() => Processor.ReadElements(path));
        Approvals.Verify(exception.Message.Replace(currentDirectory, ""));
    }

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

    static IEnumerable<string> GetPaths()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers1.xml");
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers2.xml");
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers3.xml");
    }
}