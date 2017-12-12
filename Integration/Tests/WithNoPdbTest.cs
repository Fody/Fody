using Xunit;
using WithNoPdb;

public class WithNoPdbTest
{
    [Fact]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.True(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}