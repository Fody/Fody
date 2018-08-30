using System;
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
#if(net472)
        ApprovalTests.Approvals.Verify(verify);
#endif
    }

    static string GetAssemblyPath()
    {
        var assembly = typeof(FactAttribute).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }
}