using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class ProjectWeaversReaderTests
{
    [Test]
    public void Simple()
    {
        
        var projectPathFinder = Substitute.For<ProjectWeaversFinder>();
        projectPathFinder.ConfigFiles.AddRange(GetPaths());
        var weaversReader = new ProjectWeaversReader
                                {
                                    ProjectWeaversFinder = projectPathFinder
                                };
        weaversReader.Execute();
        var weavers = weaversReader.Weavers;
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
        var currentDirectory = AssemblyLocation.CurrentDirectory();
        yield return Path.Combine(currentDirectory, @"ProjectWeaversReaderTests\FodyWeavers1.xml");
        yield return Path.Combine(currentDirectory, @"ProjectWeaversReaderTests\FodyWeavers2.xml");
        yield return Path.Combine(currentDirectory, @"ProjectWeaversReaderTests\FodyWeavers3.xml");
    }   
}