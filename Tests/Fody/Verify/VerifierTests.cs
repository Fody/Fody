using System.IO;
using System.Linq;
using Xunit;

public class VerifierTests : TestBase
{
    [Fact]
    public void ExtractVerifyAssemblyFromConfig_NotExists()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Verify\VerifierTests_NoVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(filePath)
        });
        Assert.False(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodels_NotExists()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Verify\VerifierTests_NoVerifyIgnoreCodes.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(filePath)
        });
        Assert.Empty(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodels_WithCodeMultiple()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Verify\VerifierTests_VerifyIgnoreCodes_Multiple.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(filePath)
        }).ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
        Assert.Contains("myignorecode2", verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodels_WithCodeSingle()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Verify\VerifierTests_VerifyIgnoreCodes_Single.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(filePath)
        }).ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_WithTrue()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Verify\VerifierTests_WithTrueVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(filePath)
        });
        Assert.True(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_WithFalse()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Verify\VerifierTests_WithFalseVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(filePath)
        });
        Assert.False(verifyAssembly);
    }
}