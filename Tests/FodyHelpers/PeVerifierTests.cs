using System;
using System.IO;
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
        var verify = PeVerifier.Verify(GetAssemblyPath(), GetIgnoreCodes(), out var output);
        Assert.True(verify);
        Assert.NotNull(output);
        Assert.NotEmpty(output);
    }

    [Fact]
    public void Same_assembly_should_not_throw()
    {
        var assemblyPath = GetAssemblyPath().ToLowerInvariant();
        Directory.CreateDirectory("temp");
        var newAssemblyPath = Path.GetFullPath("temp/temp.dll");
        File.Copy(assemblyPath, newAssemblyPath, true);
        PeVerifier.ThrowIfDifferent(assemblyPath, newAssemblyPath,
            ignoreCodes: GetIgnoreCodes());
        File.Delete(newAssemblyPath);
    }

    static string[] GetIgnoreCodes()
    {
        return new[] { "0x80070002", "0x80131869" };
    }

    [Fact]
    public void Invalid_assembly_should_throw()
    {
        var assemblyPath = GetAssemblyPath().ToLowerInvariant();
        Directory.CreateDirectory("temp");
        var newAssemblyPath = Path.GetFullPath("temp/temp.dll");
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