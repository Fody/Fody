using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

public class VerifierTests : TestBase
{
    [Fact]
    public void StaticPathResolution()
    {
        Assert.True(Verifier.FoundPeVerify);
        Assert.True(Directory.Exists(Verifier.WindowsSdkDirectory));
        Assert.True(File.Exists(Verifier.PeverifyPath));
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_NotExists()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\PeVerifierTests_NoVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
        {
            filePath
        });
        Assert.False(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodels_NotExists()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\PeVerifierTests_NoVerifyIgnoreCodes.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new List<string>
        {
            filePath
        });
        Assert.Empty(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodels_WithCodeMultiple()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\PeVerifierTests_VerifyIgnoreCodes_Multiple.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new List<string>
        {
            filePath
        })
            .ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
        Assert.Contains("myignorecode2", verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyIgnoreCodels_WithCodeSingle()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\PeVerifierTests_VerifyIgnoreCodes_Single.xml");
        var verifyAssembly = Verifier.ExtractVerifyIgnoreCodesConfigs(new List<string>
        {
            filePath
        })
            .ToList();
        Assert.Contains("myignorecode1", verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_WithTrue()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\PeVerifierTests_WithTrueVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
        {
            filePath
        });
        Assert.True(verifyAssembly);
    }

    [Fact]
    public void ExtractVerifyAssemblyFromConfig_WithFalse()
    {
        var filePath = Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\PeVerifierTests_WithFalseVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
        {
            filePath
        });
        Assert.False(verifyAssembly);
    }
}