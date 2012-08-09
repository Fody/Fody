using NUnit.Framework;

[TestFixture]
public class CurrentVersionTests
{
    [Test]
    public void Simple()
    {
        Assert.IsNotNull(CurrentVersion.Version);
    }
}