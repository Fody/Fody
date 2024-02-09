using System;
using System.IO;
using System.Threading.Tasks;
using Fody;
using Mono.Cecil;
using VerifyXunit;
using Xunit;

public class WeaverTestHelperTests
{
    [Fact]
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

    [Fact]
    public Task WeaverUsingSymbols()
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
        Assert.True(module.HasSymbols);

        return Verify(result);
    }
}