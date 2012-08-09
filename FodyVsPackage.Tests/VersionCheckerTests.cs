using NUnit.Framework;

[TestFixture]
public class VersionCheckerTests
{
    [Test]
    public void Simple()
    {
        var location = typeof (AssemblyLocation).Assembly.CodeBase.Replace("file:///", "");
        Assert.IsFalse(VersionChecker.IsVersionNewer(location));
    }
}