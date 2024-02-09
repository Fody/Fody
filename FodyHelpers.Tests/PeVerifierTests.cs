using System;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Fody;
using Mono.Cecil;
using Xunit;

// ReSharper disable UnusedVariable
public class PeVerifierTests
{
    string assemblyPath = "FodyHelpers.Tests.dll";
    [Fact]
    public void StaticPathResolution() =>
        Assert.True(PeVerifier.FoundPeVerify);

    [Fact]
    public void Should_verify_current_assembly()
    {
        var verify = PeVerifier.Verify(assemblyPath, GetIgnoreCodes(), out var output);
        Assert.True(verify);
        Assert.NotNull(output);
    }

    [Fact]
    public void Same_assembly_should_not_throw()
    {
        Directory.CreateDirectory("temp");
        var newAssemblyPath = Path.GetFullPath("temp/temp.dll");
        File.Copy(assemblyPath, newAssemblyPath, true);
        PeVerifier.ThrowIfDifferent(assemblyPath, newAssemblyPath,
            ignoreCodes: GetIgnoreCodes());
        File.Delete(newAssemblyPath);
    }

    static string[] GetIgnoreCodes() =>
        ["0x80070002", "0x80131869"];

    [Fact]
    public Task TrimLineNumbers()
    {
        var text = PeVerifier.TrimLineNumbers(
            """
            [IL]: Error: [C:\Code\net452\AssemblyToProcess.dll : UnsafeClass::MethodWithAmp][offset 0x00000002][found Native Int][expected unmanaged pointer] Unexpected type on the stack.
            [IL]: Error: [C:\Code\net452\AssemblyToProcess.dll : UnsafeClass::get_NullProperty][offset 0x00000006][found unmanaged pointer][expected unmanaged pointer] Unexpected type on the stack.
            [IL]: Error: [C:\Code\net452\AssemblyToProcess.dll : UnsafeClass::set_NullProperty][offset 0x00000001] Unmanaged pointers are not a verifiable type.
            3 Error(s) Verifying C:\Code\Fody\net452\AssemblyToProcess.dll
            """);
        return Verifier.Verify(text);
    }

    [Fact]
    public void Invalid_assembly_should_throw()
    {
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
}