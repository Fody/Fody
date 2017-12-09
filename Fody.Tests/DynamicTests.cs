using NUnit.Framework;

[TestFixture]
public class DynamicTests
{
    public string Foo;
    [Test]
    [Explicit]
    public void NoProperty()
    {
        dynamic x = "aString";
        x.Foo = "aString";
    }
    [Test]
    [Explicit]
    public void WrongType()
    {
        dynamic x = new DynamicTests();
        x.Foo = 1;
    }
}