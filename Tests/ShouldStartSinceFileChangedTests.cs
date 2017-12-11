using Moq;
using NUnit.Framework;

[TestFixture]
public class ShouldStartSinceFileChangedTests
{
    [Test]
    public void NotProcessedByFody()
    {
        var processor = new Processor
            {
                Logger = new Mock<BuildLogger>().Object,
                AssemblyFilePath = typeof (string).Assembly.CodeBase.Replace("file:///", "")
            };
        Assert.IsTrue(processor.ShouldStartSinceFileChanged());
    }

    [Test]
    public void ProcessedByFody()
    {
        var processor = new Processor
            {
                Logger = new Mock<BuildLogger>().Object,
                AssemblyFilePath = GetType().Assembly.CodeBase.Replace("file:///", "")
            };
        Assert.IsFalse(processor.ShouldStartSinceFileChanged());
    }
}

public interface ProcessedByFody
{

}