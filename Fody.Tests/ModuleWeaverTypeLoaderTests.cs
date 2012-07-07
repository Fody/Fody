using System.IO;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class LoadInstanceTests
{
    [Test]
    public void Simple()
    {
        var weaverFilesEnumerator = Substitute.For<AddinFilesEnumerator>();
        var fullPath = Path.GetFullPath(@"Packages\SampleTask.Fody.1.0.0.0\SampleTask.Fody.dll");
        weaverFilesEnumerator.FindAddinAssembly("").ReturnsForAnyArgs(fullPath);

        var taskTypeLoader = new WeaverAssemblyPathFinder
                                 {
                                     ContainsTypeChecker = Substitute.For<ContainsTypeChecker>(),
                                     AddinFilesEnumerator = weaverFilesEnumerator
                                 };
        taskTypeLoader.FindAssemblyPath("SampleTask");
    }
}