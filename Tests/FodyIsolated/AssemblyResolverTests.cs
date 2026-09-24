using System.IO;
using System.Linq;
using DummyAssembly;

public class AssemblyResolverTests
{
    ILogger logger = new MockBuildLogger();

    [Test]
    public async Task ShouldFindReferenceByAssemblyName()
    {
        var assemblyPath = Path.GetTempFileName();
        try
        {
            var assembly = typeof(Class1).Assembly;
            File.Copy(assembly.Location, assemblyPath, true);

            var resolver = new AssemblyResolver(logger, new[] {assemblyPath});
            using var resolvedAssembly = resolver.Resolve(assembly.GetName().Name!);
            await Assert.That(resolvedAssembly!.FullName).IsEqualTo(assembly.FullName);
        }
        finally
        {
            File.Delete(assemblyPath);
        }
    }

    [Test]
    public async Task ShouldReturnNullWhenTheAssemblyIsNotFound()
    {
        var resolver = new AssemblyResolver(logger, Enumerable.Empty<string>());
        await Assert.That(resolver.Resolve("SomeNonExistingAssembly")).IsNull();
    }

    [Test]
    public async Task ShouldGuessTheAssemblyNameFromTheFileNameIfTheAssemblyCannotBeLoaded()
    {
        var resolver = new AssemblyResolver(logger, new[] {@"Fody\BadAssembly.dll"});
        await Assert.That(() => resolver.Resolve("BadAssembly")).ThrowsException();
    }
}
