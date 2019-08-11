using System;
using System.IO;
using Fody;
using Mono.Cecil;
using Xunit;
using Xunit.Abstractions;

public class WeaverTestHelperTests :
    XunitLoggingBase
{
    [Fact]
    public void Run()
    {
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun("DummyAssembly.dll");
        Verify(result);
    }

    static void Verify(TestResult result)
    {
        ObjectApprover.Verify(new
            {
                result.Errors,
                result.Messages,
                result.Warnings,
                result.AssemblyPath,
                result.Assembly.FullName
            },
            ScrubCurrentDirectory);
    }

    static string ScrubCurrentDirectory(string s)
    {
        return s.Replace(@"\\", @"\").Replace(Environment.CurrentDirectory, "");
    }

    [Fact]
    public void WithCustomAssemblyName()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(
            assemblyPath: assemblyPath,
            assemblyName: "NewName");
        Verify(result);
    }

    [Fact]
    public void WeaverUsingSymbols()
    {
        var start = DateTime.Now;
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new WeaverUsingSymbols();
        var result = weaver.ExecuteTestRun(assemblyPath, writeSymbols:true);
        var module = ModuleDefinition.ReadModule(assemblyPath,new ReaderParameters {ReadSymbols = true});
        Assert.True(module.HasSymbols);

        Verify(result);
    }

    public WeaverTestHelperTests(ITestOutputHelper output) :
        base(output)
    {
    }
}