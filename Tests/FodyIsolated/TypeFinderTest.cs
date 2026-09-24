public class TypeFinderTest
{
    [Test]
    public async Task Valid()
    {
        var assembly = typeof(InnerWeaver).Assembly;
        assembly.FindType("ModuleReader");
    }

    [Test]
    public async Task NoTypeInAssembly()
    {
        var assembly = GetType().Assembly;
        await Assert.That(assembly.FindType("ModuleWeaver")).IsNull();
    }
}