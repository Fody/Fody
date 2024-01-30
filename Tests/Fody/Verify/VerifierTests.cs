using System.Linq;

public class VerifierTests
{
    [Fact]
    public void ExtractVerifyAssemblyFromConfig_NotExists()
    {
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_NoVerifyAssembly.xml")
        });
        Assert.False(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodes_NotExists()
    {
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_NoVerifyIgnoreCodes.xml")
        });
        Assert.Empty(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodes_WithCodeMultiple()
    {
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_VerifyIgnoreCodes_Multiple.xml")
        }).ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
        Assert.Contains("myignorecode2", verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodes_WithCodeSingle()
    {
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_VerifyIgnoreCodes_Single.xml")
        }).ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_WithTrue()
    {
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_WithTrueVerifyAssembly.xml")
        });
        Assert.True(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_WithFalse()
    {
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_WithFalseVerifyAssembly.xml")
        });
        Assert.False(verifyAssembly);
    }
}