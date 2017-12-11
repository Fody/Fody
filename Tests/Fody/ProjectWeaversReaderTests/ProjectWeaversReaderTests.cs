using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class ProjectWeaversReaderTests
{
    [Test]
    public void Simple()
    {
        var processor = new Processor
                        {
                            ConfigFiles = GetPaths().ToList()
                        };
        processor.ReadProjectWeavers();
        var weavers = processor.Weavers;
        Assert.AreEqual(3, weavers.Count);
        Assert.AreEqual("SampleTask1", weavers[0].AssemblyName);
        Assert.AreEqual("<SampleTask1 MyProperty1=\"PropertyValue2Overwrite\" />", weavers[0].Element);
        Assert.AreEqual("SampleTask2", weavers[1].AssemblyName);
        Assert.AreEqual("<SampleTask2 MyProperty2=\"PropertyValue2\" />", weavers[1].Element);
        Assert.AreEqual("SampleTask3", weavers[2].AssemblyName);
        Assert.AreEqual("<SampleTask3 MyProperty3=\"PropertyValue3\" />", weavers[2].Element);
    }

    static IEnumerable<string> GetPaths()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers1.xml");
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers2.xml");
        yield return Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\FodyWeavers3.xml");
    }
}