using System;
using System.Runtime.InteropServices;
using ApprovalTests;
using ApprovalTests.Namers;
using DummyAssembly;
using Fody;
using Xunit;
// ReSharper disable UnusedVariable

public class IldasmTests :
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
        disposable = NamerFactory.AsEnvironmentSpecificTest(() => "Debug" + ApprovalResults.GetDotNetRuntime(true, RuntimeInformation.FrameworkDescription));
#else
        disposable = NamerFactory.AsEnvironmentSpecificTest(() => "Release" + ApprovalResults.GetDotNetRuntime(true, RuntimeInformation.FrameworkDescription));
#endif
    }

    [Fact]
    public void VerifyMethod()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath(), "DummyAssembly.Class1::Method");
        using (UniqueForRuntime())
        {
            Approvals.Verify(verify);
        }
    }

    static IDisposable UniqueForRuntime(bool throwOnError = true)
    {
        return NamerFactory.AsEnvironmentSpecificTest(() => ApprovalResults.GetDotNetRuntime(throwOnError, RuntimeInformation.FrameworkDescription));
    }

    [Fact]
    public void Verify()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath());
        using (UniqueForRuntime())
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