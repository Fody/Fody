using NUnit.Framework;
using WithNugetWeavers;

[TestFixture]
public class WithNugetWeaversTest
{
    [Test]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.IsTrue(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}