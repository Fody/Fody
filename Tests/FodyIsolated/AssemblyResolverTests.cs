using System;
using System.IO;
using System.Linq;
using DummyAssembly;
using Xunit;

public class AssemblyResolverTests : TestBase
{
    [Fact]
    public void ShouldFindReferenceByAssemblyName()
    {
        var assemblyPath = Path.GetTempFileName();
        try
        {
            var assembly = typeof(Class1).Assembly;
            File.Copy(assembly.Location, assemblyPath, true);

            var resolver = new AssemblyResolver(new NullLogger(), new[] {assemblyPath});
            using (var resolvedAssembly = resolver.Resolve(assembly.GetName().Name))
            {
                Assert.Equal(assembly.FullName, resolvedAssembly.FullName);
            }
        }
        finally
        {
            File.Delete(assemblyPath);
        }
    }

    [Fact]
    private void ShouldReturnNullWhenTheAssemblyIsNotFound()
    {
        var resolver = new AssemblyResolver(new NullLogger(), Enumerable.Empty<string>());
        Assert.Null(resolver.Resolve("SomeNonExistingAssembly"));
    }

    [Fact]
    private void ShouldGuessTheAssemblyNameFromTheFileNameIfTheAssemblyCannotBeLoaded()
    {
        var resolver = new AssemblyResolver(new NullLogger(), new[] {Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\BadAssembly.dll")});
        Assert.ThrowsAny<Exception>(() => resolver.Resolve("BadAssembly"));
    }
}