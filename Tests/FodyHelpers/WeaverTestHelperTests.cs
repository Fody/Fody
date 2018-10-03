using System;
using System.Collections.Generic;
using System.IO;
using Fody;
using ObjectApproval;
using Xunit;
// ReSharper disable UnusedVariable

//using Xunit;

#pragma warning disable 618
public class WeaverTestHelperTests : TestBase
{
    [Fact]
    public void Run()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(assemblyPath);
        Verify(result);
    }

    static void Verify(TestResult result)
    {
        ObjectApprover.VerifyWithJson(new
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
        return s.Replace(@"\\", @"\").Replace(CodeBaseLocation.CurrentDirectory, "");
    }

    [Fact]
    public void WithCustomAssemblyName()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
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
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new WeaverUsingSymbols();
        var result = weaver.ExecuteTestRun(assemblyPath);

        var symbolsPath = Path.ChangeExtension(result.AssemblyPath, ".pdb");
        var symbolsFileInfo = new FileInfo(symbolsPath);

        Assert.True(symbolsFileInfo.Exists);
        Assert.True(start <= symbolsFileInfo.LastWriteTime);

        Verify(result);
    }
}

public class TargetWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var type = FindType("System.Boolean");
        Assert.NotNull(type);

        type = FindType("Boolean");
        Assert.NotNull(type);

        var result = TryFindType("System.Boolean", out type);
        Assert.True(result);
        Assert.NotNull(type);

        result = TryFindType("Boolean", out type);
        Assert.True(result);
        Assert.NotNull(type);

        result = TryFindType("DDD", out type);
        Assert.False(result);
        Assert.Null(type);
    }

    public override bool ShouldCleanReference => true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
    }
}