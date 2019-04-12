using Xunit;
using Xunit.Abstractions;

public class TypeFinderTest :
    XunitLoggingBase
{
    [Fact]
    public void Valid()
    {
        var assembly = typeof(InnerWeaver).Assembly;
        assembly.FindType("ModuleReader");
    }

    [Fact]
    public void NoTypeInAssembly()
    {
        var assembly = GetType().Assembly;
        Assert.Null(assembly.FindType("ModuleWeaver"));
    }

    public TypeFinderTest(ITestOutputHelper output) : 
        base(output)
    {
    }
}