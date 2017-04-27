using NUnit.Framework;
using WithNoPdb;

[TestFixture]
public class WithNoPdbTest
{
    [Test]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.IsTrue(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}