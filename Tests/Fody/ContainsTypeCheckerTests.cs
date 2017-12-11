using Xunit;

public class ContainsTypeCheckerTests : TestBase
{
    [Fact]
    public void Exists()
    {
        var checker = new ContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "ContainsTypeCheckerTests");
        Assert.True(check);
    }

    [Fact]
    public void NotExists()
    {
        var checker = new ContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "BadType");
        Assert.False(check);
    }
}