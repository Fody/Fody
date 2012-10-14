using Moq;
using NUnit.Framework;

[TestFixture]
public class FileChangedCheckerTests
{
    [Test]
    public void Simple()
    {
        var processor = new Processor
            {
                ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object,
                Logger = new Mock<BuildLogger>().Object,
                AssemblyFilePath = GetType().Assembly.Location
            };
        Assert.IsTrue(processor.ShouldStartSinceFileChanged());
    }
}