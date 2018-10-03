using System;
using ApprovalTests;
using ApprovalTests.Namers;
using Fody;
using Xunit;
// ReSharper disable UnusedVariable

#pragma warning disable 618
public class IldasmTests : TestBase
{
    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(Ildasm.FoundIldasm);
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
        var assembly = typeof(FactAttribute).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }
}