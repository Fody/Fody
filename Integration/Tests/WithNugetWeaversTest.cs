using Xunit;
using WithNugetWeavers;

public class WithNugetWeaversTest
{
    [Fact]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.True(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}