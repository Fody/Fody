using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class LoadInstanceTests
{
    [Test]
    public void Simple()
    {
        var addinFilesEnumeratorMock = new Mock<Processor>();
        var fullPath = Path.GetFullPath(@"Packages\SampleTask.Fody.1.0.0.0\SampleTask.Fody.dll");
        addinFilesEnumeratorMock
            .Setup(x => x.FindAddinAssembly(It.IsAny<string>()))
            .Returns(fullPath);
        var innerWeavingTask = addinFilesEnumeratorMock.Object;
        innerWeavingTask.ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object;

        innerWeavingTask.FindAssemblyPath("SampleTask");
    }
}