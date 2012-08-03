using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class LoadInstanceTests
{
    [Test]
    public void Simple()
    {
        var mock = new Mock<Processor>();
        var fullPath = Path.GetFullPath(@"Packages\SampleTask.Fody.1.0.0.0\SampleTask.Fody.dll");
        mock
            .Setup(x => x.FindAddinAssembly(It.IsAny<string>()))
            .Returns(fullPath);
        var processor = mock.Object;
        processor.ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object;

        processor.FindAssemblyPath("SampleTask");
    }
}