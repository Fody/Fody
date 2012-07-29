using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class FileChangedCheckerTests
{
    [Test]
    public void Simple()
    {
        var changedChecker = new FileChangedChecker
                                 {
                                     ContainsTypeChecker = Substitute.For<ContainsTypeChecker>(),
                                     Logger = Substitute.For<ILogger>(),
                                     AssemblyPath = GetType().Assembly.Location
                                 };
        Assert.IsTrue(changedChecker.ShouldStart());
    }
}