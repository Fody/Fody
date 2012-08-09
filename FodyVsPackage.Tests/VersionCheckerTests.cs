using NUnit.Framework;

[TestFixture]
public class VersionCheckerTests
{
    [Test]
    public void InNotNewer()
    {
        var location = typeof (AssemblyLocation).Assembly.CodeBase.Replace("file:///", "");
        Assert.IsFalse(VersionChecker.IsVersionNewer(location));
    }
    [Test]
    public void IsNewer()
    {
        var location = GetType().Assembly.CodeBase.Replace("file:///", "");
        Assert.IsTrue(VersionChecker.IsVersionNewer(location));
    }
}