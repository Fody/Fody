using Xunit;
using Xunit.Abstractions;

public class FodyVersionTests :
    XunitLoggingBase
{
    [Fact]
    public void FindFodyHelpersReference()
    {
        Assert.Equal("FodyHelpers", FodyVersion.FindFodyHelpersReference(GetType().Assembly).Name);
    }

    public FodyVersionTests(ITestOutputHelper output) :
        base(output)
    {
    }
}