using System;
using System.IO;
using System.Threading.Tasks;
using Fody;
using Mono.Cecil;
using VerifyTUnit;
using TestResult = Fody.TestResult;

// tests share the temp folders
[NotInParallel]
public class WeaverTestHelperTests
{
    [Test]
    public Task Run()
    {
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun("DummyAssembly.dll");
        return Verify(result);
    }

    static Task Verify(TestResult result) =>
        Verifier.Verify(new
        {
            result.Errors,
            result.Messages,
            result.Warnings,
            result.AssemblyPath,
            result.Assembly.FullName
        });

    [Test]
    public Task WithCustomAssemblyName()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(
            assemblyPath: assemblyPath,
            assemblyName: "NewName");
        return Verify(result);
    }

    [Test]
    public Task WithCustomExeAssemblyName()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyExeAssembly.exe");
        try
        {
            var weaver = new TargetWeaver();
            var result = weaver.ExecuteTestRun(
                assemblyPath: assemblyPath,
                assemblyName: "NewName");
            return Verify(result);
        }
        catch (BadImageFormatException) when (AppContext.TargetFrameworkName!.StartsWith(".NETCoreApp"))
        {
            // The .NET Core DummyExeAssembly.exe file makes Mono.Cecil throw a BadImageFormatException ¯\_(ツ)_/¯
            return Task.CompletedTask;
        }
    }

    [Test]
    public async Task WeaverUsingSymbols()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new WeaverUsingSymbols();
        var result = weaver.ExecuteTestRun(assemblyPath, writeSymbols: true);
        var module = ModuleDefinition.ReadModule(
            assemblyPath,
            new()
            {
                ReadSymbols = true
            });
        await Assert.That(module.HasSymbols).IsTrue();

        await Verify(result);
    }
}