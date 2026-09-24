using System.Linq;

public class VerifierTests
{
    [Test]
    public async Task ExtractVerifyAssemblyFromConfig_NotExists()
    {
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_NoVerifyAssembly.xml")
        });
        await Assert.That(verifyAssembly).IsFalse();
    }

    [Test]
    public async Task ExtractVerifyIgnoreCodes_NotExists()
    {
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_NoVerifyIgnoreCodes.xml")
        });
        await Assert.That(verifyAssembly).IsEmpty();
    }

    [Test]
    public async Task ExtractVerifyIgnoreCodes_WithCodeMultiple()
    {
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_VerifyIgnoreCodes_Multiple.xml")
        }).ToList();
        await Assert.That(verifyAssembly).Contains("myignorecode1");
        await Assert.That(verifyAssembly).Contains("myignorecode2");
    }

    [Test]
    public async Task ExtractVerifyIgnoreCodes_WithCodeSingle()
    {
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_VerifyIgnoreCodes_Single.xml")
        }).ToList();
        await Assert.That(verifyAssembly).Contains("myignorecode1");
    }

    [Test]
    public async Task ExtractVerifyAssemblyFromConfig_WithTrue()
    {
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_WithTrueVerifyAssembly.xml")
        });
        await Assert.That(verifyAssembly).IsTrue();
    }

    [Test]
    public async Task ExtractVerifyAssemblyFromConfig_WithFalse()
    {
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new[]
        {
            new WeaverConfigFile(@"Fody\Verify\VerifierTests_WithFalseVerifyAssembly.xml")
        });
        await Assert.That(verifyAssembly).IsFalse();
    }
}