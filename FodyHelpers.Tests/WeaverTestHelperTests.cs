using System;
using System.IO;
using System.Threading.Tasks;
using Fody;
using Mono.Cecil;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class WeaverTestHelperTests
{
    [Fact]
    public Task Run()
    {
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun("DummyAssembly.dll");
        return Verify(result);
    }

    Task Verify(TestResult result)
    {
        return Verifier.Verify(new
        {
            result.Errors,
            result.Messages,
            result.Warnings,
            result.AssemblyPath,
            result.Assembly.FullName
        });
    }

    [Fact]
    public Task WithCustomAssemblyName()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(
            assemblyPath: assemblyPath,
            assemblyName: "NewName");
        return Verify(result);
    }

    [Fact]
    public Task WeaverUsingSymbols()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new WeaverUsingSymbols();
        var result = weaver.ExecuteTestRun(assemblyPath, writeSymbols: true);
        var module = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters {ReadSymbols = true});
        Assert.True(module.HasSymbols);

        return Verify(result);
    }
}