using System;
using ApprovalTests;
using ApprovalTests.Namers;
using DummyAssembly;
using Fody;
using Xunit;
// ReSharper disable UnusedVariable

#pragma warning disable 618
public class IldasmTests :
    TestBase,
    IDisposable
{
    IDisposable disposable;

    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(Ildasm.FoundIldasm);
    }

    public IldasmTests()
    {
#if DEBUG
        disposable = NamerFactory.AsEnvironmentSpecificTest(() => "Debug" + ApprovalResults.GetDotNetRuntime(true));
#else
        disposable = NamerFactory.AsEnvironmentSpecificTest(() => "Release" + ApprovalResults.GetDotNetRuntime(true));
#endif
    }

    [Fact]
    public void VerifyMethod()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath(), "DummyAssembly.Class1::Method");
        using (ApprovalResults.UniqueForRuntime())
        {
            Approvals.Verify(verify);
        }
    }

    [Fact]
    public void Verify()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath());
        using (ApprovalResults.UniqueForRuntime())
        {
            Approvals.Verify(verify);
        }
    }

    static string GetAssemblyPath()
    {
        var assembly = typeof(Class1).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }

    public void Dispose()
    {
        disposable.Dispose();
    }
}