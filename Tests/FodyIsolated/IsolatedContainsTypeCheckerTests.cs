using NUnit.Framework;

[TestFixture]
public class IsolatedContainsTypeCheckerTests
{
    [Test]
    public void Exists()
    {
        var checker = new IsolatedContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "IsolatedContainsTypeCheckerTests");
        Assert.IsTrue(check);
    }

    [Test]
    public void NotExists()
    {
        var checker = new IsolatedContainsTypeChecker();
        var check = checker.Check(GetType().Assembly.Location, "BadType");
        Assert.IsFalse(check);
    }
}