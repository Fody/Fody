using System.IO;
using System.Linq;

public class ModuleReaderTests
{
    [Test]
    public async Task WithSymbols()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var result = InnerWeaver.ReadModule(assemblyPath, new AssemblyResolver(new MockBuildLogger(), Enumerable.Empty<string>()));
        await Assert.That(result.module).IsNotNull();
        await Assert.That(result.hasSymbols).IsTrue();
    }

    [Test]
    public async Task NoSymbols()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "AssemblyWithNoSymbols.dll");
        var result = InnerWeaver.ReadModule(assemblyPath, new AssemblyResolver(new MockBuildLogger(), Enumerable.Empty<string>()));
        await Assert.That(result.module).IsNotNull();
        await Assert.That(result.hasSymbols).IsFalse();
    }
}