using Xunit;

public class IsolatedContainsTypeCheckerTests : TestBase
{
    [Fact]
    public void Exists()
    {
        var checker = new IsolatedContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "IsolatedContainsTypeCheckerTests");
        Assert.True(check);
    }

    [Fact]
    public void NotExists()
    {
        var checker = new IsolatedContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "BadType");
        Assert.False(check);
    }
}