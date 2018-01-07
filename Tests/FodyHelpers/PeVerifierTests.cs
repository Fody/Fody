using System;
using System.IO;
using System.Linq;
using Fody;
using Mono.Cecil;
using Xunit;

#pragma warning disable 618
public class PeVerifierTests : TestBase
{
    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(PeVerifier.FoundPeVerify);
    }

    [Fact]
    public void Should_verify_current_assembly()
    {
        var verify = PeVerifier.Verify(GetAssemblyPath(), Enumerable.Empty<string>(), out var output);
        Assert.True(verify);
        Assert.NotNull(output);
        Assert.NotEmpty(output);
    }

    [Fact]
    public void Same_assembly_should_not_throw()
    {
        var assemblyPath = GetAssemblyPath().ToLowerInvariant();
        var newAssemblyPath = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssemblyPath, true);
#if (NET46)
        PeVerifier.ThrowIfDifferent(assemblyPath, newAssemblyPath);
#else
        PeVerifier.ThrowIfDifferent(assemblyPath, newAssemblyPath,
            ignoreCodes: new[] {"0x80070002", "0x80131869"});
#endif
        File.Delete(newAssemblyPath);
    }

    [Fact]
    public void Invalid_assembly_should_throw()
    {
        var assemblyPath = GetAssemblyPath().ToLowerInvariant();
        var newAssemblyPath = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssemblyPath, true);
        using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath))
        {
            moduleDefinition.AssemblyReferences.Clear();
            moduleDefinition.Write(newAssemblyPath);
        }

        Assert.Throws<Exception>(() => PeVerifier.ThrowIfDifferent(assemblyPath, newAssemblyPath));
        File.Delete(newAssemblyPath);
    }

    static string GetAssemblyPath()
    {
        var assembly = typeof(TestBase).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }
}