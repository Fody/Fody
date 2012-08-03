using Moq;
using NUnit.Framework;

[TestFixture]
public class FileChangedCheckerTests
{
    [Test]
    public void Simple()
    {
        var changedChecker = new Processor
                                 {
                                     ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object,
                                     Logger = new Mock<BuildLogger>().Object,
                                     AssemblyPath = GetType().Assembly.Location
                                 };
        Assert.IsTrue(changedChecker.ShouldStartSinceFileChanged());
    }
}