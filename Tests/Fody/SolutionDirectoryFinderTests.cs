using System.IO;

public class SolutionDirectoryFinderTests
{
    [Test]
    public async Task ReturnNCrunchSolutionWhenPresent()
    {
        var result = SolutionDirectoryFinder.Find("Foo", "Baz", "Bar");
        await Assert.That(result).IsEqualTo("Baz");
    }

    [Test]
    public async Task ReturnSolutionWhenPresent()
    {
        var result = SolutionDirectoryFinder.Find("Foo", null, "Bar");
        await Assert.That(result).IsEqualTo("Foo");
    }

    [Test]
    public async Task ReturnProjectParentWhenSolutionIsEmpty()
    {
        var result = SolutionDirectoryFinder.Find(null, null, Environment.CurrentDirectory);
        await Assert.That(result).IsEqualTo(Path.GetDirectoryName(Environment.CurrentDirectory));
    }

    [Test]
    public async Task IgnoreUndefinedSolution()
    {
        var result = SolutionDirectoryFinder.Find("*Undefined*", null, Environment.CurrentDirectory);
        await Assert.That(result).IsEqualTo(Path.GetDirectoryName(Environment.CurrentDirectory));
    }
}