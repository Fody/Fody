using NUnit.Framework;

[TestFixture]
public class TypeFinderTest
{

    [Test]
    public void Valid()
    {
        var assembly = typeof(ModuleReader).Assembly;
        assembly.FindType("ModuleReader");
    }

    [Test]
    public void NoTypeInAssembly()
    {
        var assembly = GetType().Assembly;
        Assert.IsNull(assembly.FindType("ModuleWeaver"));
    }
}