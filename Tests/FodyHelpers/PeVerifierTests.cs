using System;
using System.Linq;
using Fody;
using Xunit;

#pragma warning disable 618
public class PeVerifierTests : TestBase
{
    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(PeVerifier.FoundPeVerify);
    }

    [Fact]
    public void Should_verify_current_assembly()
    {
        var verify = PeVerifier.Verify(GetAssemblyPath(), Enumerable.Empty<string>(),out var output);
        Assert.True(verify);
        Assert.NotNull(output);
        Assert.NotEmpty(output);
    }

    static string GetAssemblyPath()
    {
        var assembly = typeof(TestBase).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }
}