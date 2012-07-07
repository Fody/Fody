using NUnit.Framework;

[TestFixture]
public class ObjectTypeNameTests
{

    [Test]
    public void Simple()
    {
        Assert.AreEqual("System.String, mscorlib", "".GetTypeName());
    }
}