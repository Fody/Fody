public class TypeFinderTest
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
}