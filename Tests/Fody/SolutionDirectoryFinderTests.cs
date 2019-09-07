using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

public class SolutionDirectoryFinderTests :
    XunitApprovalBase
{
    [Fact]
    public void ReturnNCrunchSolutionWhenPresent()
    {
        var result = SolutionDirectoryFinder.Find("Foo", "Baz", "Bar");
        Assert.Equal("Baz", result);
    }

    [Fact]
    public void ReturnSolutionWhenPresent()
    {
        var result = SolutionDirectoryFinder.Find("Foo", null, "Bar");
        Assert.Equal("Foo", result);
    }

    [Fact]
    public void ReturnProjectParentWhenSolutionIsEmpty()
    {
        var result = SolutionDirectoryFinder.Find(null, null, Environment.CurrentDirectory);
        Assert.Equal(Path.GetDirectoryName(Environment.CurrentDirectory), result);
    }

    [Fact]
    public void IgnoreUndefinedSolution()
    {
        var result = SolutionDirectoryFinder.Find("*Undefined*", null, Environment.CurrentDirectory);
        Assert.Equal(Path.GetDirectoryName(Environment.CurrentDirectory), result);
    }

    public SolutionDirectoryFinderTests(ITestOutputHelper output) : base(output)
    {
    }
}