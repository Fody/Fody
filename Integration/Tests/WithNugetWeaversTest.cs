using Xunit;
using WithNugetWeavers;

public class WithNugetWeaversTest
{
    [Test]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.IsTrue(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}