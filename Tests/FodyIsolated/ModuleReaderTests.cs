using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

public class ModuleReaderTests :
    XunitApprovalBase
{
    [Fact]
    public void WithSymbols()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var result = InnerWeaver.ReadModule(assemblyPath, new AssemblyResolver());
        Assert.NotNull(result.module);
        Assert.True(result.hasSymbols);
    }

    [Fact]
    public void NoSymbols()
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "AssemblyWithNoSymbols.dll");
        var result = InnerWeaver.ReadModule(assemblyPath, new AssemblyResolver());
        Assert.NotNull(result.module);
        Assert.False(result.hasSymbols);
    }

    public ModuleReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}