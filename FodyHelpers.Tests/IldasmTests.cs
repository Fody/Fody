using System;
using System.Threading.Tasks;
using VerifyXunit;
using DummyAssembly;
using Fody;
using VerifyTests;
using Xunit;

[UsesVerify]
public class IldasmTests
{
    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(Ildasm.FoundIldasm);
    }

    VerifySettings verifySettings;

    public IldasmTests()
    {
        verifySettings = new VerifySettings();
        verifySettings.UniqueForAssemblyConfiguration();
        verifySettings.UniqueForRuntime();
    }

    [Fact]
    public Task VerifyMethod()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath(), "DummyAssembly.Class1::Method");
        return Verifier.Verify(verify, verifySettings);
    }

    [Fact]
    public Task VerifyDecompile()
    {
        var verify = Ildasm.Decompile(GetAssemblyPath());
        return Verifier.Verify(verify, verifySettings);
    }

    static string GetAssemblyPath()
    {
        var assembly = typeof(Class1).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }
}