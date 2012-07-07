using Fody;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class FileChangedCheckerTests
{
    [Test]
    public void Simple()
    {
        var weavingTask = new WeavingTask
                              {
                                  AssemblyPath = GetType().Assembly.Location
                              };
        var changedChecker = new FileChangedChecker
                                 {
                                     ContainsTypeChecker = Substitute.For<ContainsTypeChecker>(),
                                     Logger = Substitute.For<ILogger>(),
                                     WeavingTask = weavingTask
                                 };
        Assert.IsTrue(changedChecker.ShouldStart());
    }
}