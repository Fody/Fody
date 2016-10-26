using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class VerifierTests
{

    [Test]
    public void StaticPathResolution()
    {
        Assert.IsTrue(Verifier.FoundPeVerify);
        Assert.IsTrue(Directory.Exists(Verifier.WindowsSdkDirectory));
        Assert.IsTrue(File.Exists(Verifier.PeverifyPath));
    }

    [Test]
    public void ExtractVerifyAssemblyFromConfig_NotExists()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PeVerifierTests_NoVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
        {
            filePath
        });
        Assert.IsFalse(verifyAssembly);
    }

    [Test]
    public void ExtractVerifyIgnoreCodels_NotExists()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PeVerifierTests_NoVerifyIgnoreCodes.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new List<string>
        {
            filePath
        });
        Assert.IsEmpty(verifyAssembly);
    }

    [Test]
    public void ExtractVerifyIgnoreCodels_WithCodeMultiple()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PeVerifierTests_VerifyIgnoreCodes_Multiple.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new List<string>
        {
            filePath
        })
            .ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
        Assert.Contains("myignorecode2", verifyAssembly);
    }

    [Test]
    public void ExtractVerifyIgnoreCodels_WithCodeSingle()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PeVerifierTests_VerifyIgnoreCodes_Single.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new List<string>
        {
            filePath
        })
            .ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
    }

    [Test]
    public void ExtractVerifyAssemblyFromConfig_WithTrue()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PeVerifierTests_WithTrueVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
        {
            filePath
        });
        Assert.IsTrue(verifyAssembly);
    }

    [Test]
    public void ExtractVerifyAssemblyFromConfig_WithFalse()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PeVerifierTests_WithFalseVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
        {
            filePath
        });
        Assert.IsFalse(verifyAssembly);
    }
}