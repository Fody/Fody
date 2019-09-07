using System;
using ApprovalTests;
using DummyAssembly;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class IldasmTests :
    XunitApprovalBase
{
    IDisposable disposable;

    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(Ildasm.FoundIldasm);
    }

    public IldasmTests(ITestOutputHelper outputHelper) :
        base(outputHelper)
    {
        disposable = RuntimeNamer.BuildForRuntimeAndConfig();
    }

    [Fact]
    public void VerifyMethod()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath(), "DummyAssembly.Class1::Method");
        Approvals.Verify(verify);
    }

    [Fact]
    public void Verify()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath());
        Approvals.Verify(verify);
    }

    static string GetAssemblyPath()
    {
        var assembly = typeof(Class1).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }

    public override void Dispose()
    {
        disposable.Dispose();
        base.Dispose();
    }
}