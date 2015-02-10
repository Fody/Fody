using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class VerifierTests
{

    [Test]
    public void StaticPathResolution()
    {
        Assert.IsTrue(Verifier.foundPeVerify);
        Assert.IsTrue(Directory.Exists(Verifier.windowsSdkDirectory));
        Assert.IsTrue(File.Exists(Verifier.peverifyPath));
    }

    [Test]
    public void ExtractVerifyAssemblyFromConfig_NotExists()
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "PeVerifierTests_NoVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
                                                                         {
                                                                             filePath
                                                                         });
        Assert.IsFalse(verifyAssembly);
    }

    [Test]
    public void ExtractVerifyAssemblyFromConfig_WithTrue()
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "PeVerifierTests_WithTrueVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
                                                                         {
                                                                             filePath
                                                                         });
        Assert.IsTrue(verifyAssembly);
    }

    [Test]
    public void ExtractVerifyAssemblyFromConfig_WithFalse()
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "PeVerifierTests_WithFalseVerifyAssembly.xml");
        var verifyAssembly = Verifier.ExtractVerifyAssemblyFromConfigs(new List<string>
                                                                         {
                                                                             filePath
                                                                         });
        Assert.IsFalse(verifyAssembly);
    }
}