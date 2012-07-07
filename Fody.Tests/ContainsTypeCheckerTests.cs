using NUnit.Framework;

[TestFixture]
public class ContainsTypeCheckerTests
{
    [Test]
    public void Exists()
    {
        var checker = new ContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "ContainsTypeCheckerTests");
        Assert.IsTrue(check);
    }

    [Test]
    public void NotExists()
    {
        var checker = new ContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "BadType");
        Assert.IsFalse(check);
    }
}