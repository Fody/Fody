using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyTUnit;
using Fody;
using Mono.Cecil;

// ReSharper disable UnusedVariable
// tests share the temp folders
[NotInParallel]
public class PeVerifierTests
{
    // test framework outputs .exe, not .dll, so we need to check for both
    readonly string assemblyPath = new[] { "dll", "exe" }.Select(ext => $"FodyHelpers.Tests.{ext}").FirstOrDefault(File.Exists) ?? throw new InvalidOperationException("Test assembly does not exist");

    [Test]
    public async Task StaticPathResolution() =>
        await Assert.That(PeVerifier.FoundPeVerify).IsTrue();

    [Test]
    public async Task Should_verify_current_assembly()
    {
        var cwd = Directory.GetCurrentDirectory();
        var verify = PeVerifier.Verify(assemblyPath, GetIgnoreCodes(), out var output);
        await Assert.That(verify).IsTrue();
        await Assert.That(output).IsNotNull();
    }

    [Test]
    public async Task Same_assembly_should_not_throw()
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

    [Test]
    public async Task TrimLineNumbers()
    {
        var text = PeVerifier.TrimLineNumbers(
            """
            [IL]: Error: [C:\Code\net452\AssemblyToProcess.dll : UnsafeClass::MethodWithAmp][offset 0x00000002][found Native Int][expected unmanaged pointer] Unexpected type on the stack.
            [IL]: Error: [C:\Code\net452\AssemblyToProcess.dll : UnsafeClass::get_NullProperty][offset 0x00000006][found unmanaged pointer][expected unmanaged pointer] Unexpected type on the stack.
            [IL]: Error: [C:\Code\net452\AssemblyToProcess.dll : UnsafeClass::set_NullProperty][offset 0x00000001] Unmanaged pointers are not a verifiable type.
            3 Error(s) Verifying C:\Code\Fody\net452\AssemblyToProcess.dll
            """);
        await Verifier.Verify(text);
    }

    [Test]
    public async Task Invalid_assembly_should_throw()
    {
        Directory.CreateDirectory("temp");
        var newAssemblyPath = Path.GetFullPath("temp/temp.dll");
        File.Copy(assemblyPath, newAssemblyPath, true);
        using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath))
        {
            moduleDefinition.AssemblyReferences.Clear();
            moduleDefinition.Write(newAssemblyPath);
        }

        await Assert.That(() => PeVerifier.ThrowIfDifferent(assemblyPath, newAssemblyPath)).Throws<Exception>();
        File.Delete(newAssemblyPath);
    }
}