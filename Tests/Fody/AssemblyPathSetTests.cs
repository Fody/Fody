namespace Tests.Fody;

public class AssemblyPathSetTests
{
    [Fact]
    public void ShouldDetectEquality()
    {
        var a = new AssemblyPathSet(["foo", "bar"]);
        var b = new AssemblyPathSet(["bar", "foo", "bar"]);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ShouldDetectInequality()
    {
        var a = new AssemblyPathSet(["foo", "bar"]);
        var b = new AssemblyPathSet(["foo", "baz"]);

        Assert.NotEqual(a, b);
    }
}
