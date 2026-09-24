namespace Tests.Fody;

public class AssemblyPathSetTests
{
    [Test]
    public async Task ShouldDetectEquality()
    {
        var a = new AssemblyPathSet(["foo", "bar"]);
        var b = new AssemblyPathSet(["bar", "foo", "bar"]);

        await Assert.That(b).IsEqualTo(a);
        await Assert.That(b.GetHashCode()).IsEqualTo(a.GetHashCode());
    }

    [Test]
    public async Task ShouldDetectInequality()
    {
        var a = new AssemblyPathSet(["foo", "bar"]);
        var b = new AssemblyPathSet(["foo", "baz"]);

        await Assert.That(b).IsNotEqualTo(a);
    }
}
