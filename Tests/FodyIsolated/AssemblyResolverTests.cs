﻿using System;
using System.IO;
using System.Linq;
using DummyAssembly;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class AssemblyResolverTests :
    VerifyBase
{
    ILogger logger = new MockBuildLogger();

    [Fact]
    public void ShouldFindReferenceByAssemblyName()
    {
        var assemblyPath = Path.GetTempFileName();
        try
        {
            var assembly = typeof(Class1).Assembly;
            File.Copy(assembly.Location, assemblyPath, true);

            var resolver = new AssemblyResolver(logger, new[] {assemblyPath});
            using var resolvedAssembly = resolver.Resolve(assembly.GetName().Name);
            Assert.Equal(assembly.FullName, resolvedAssembly!.FullName);
        }
        finally
        {
            File.Delete(assemblyPath);
        }
    }

    [Fact]
    public void ShouldReturnNullWhenTheAssemblyIsNotFound()
    {
        var resolver = new AssemblyResolver(logger, Enumerable.Empty<string>());
        Assert.Null(resolver.Resolve("SomeNonExistingAssembly"));
    }

    [Fact]
    public void ShouldGuessTheAssemblyNameFromTheFileNameIfTheAssemblyCannotBeLoaded()
    {
        var resolver = new AssemblyResolver(logger, new[] {@"Fody\BadAssembly.dll"});
        Assert.ThrowsAny<Exception>(() => resolver.Resolve("BadAssembly"));
    }

    public AssemblyResolverTests(ITestOutputHelper output) :
        base(output)
    {
    }
}
