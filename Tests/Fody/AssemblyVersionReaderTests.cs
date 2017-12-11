using System.IO;
using NUnit.Framework;

[TestFixture]
public class AssemblyVersionReaderTests
{
    [Test]
    public void BadImage()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Fody\BadAssembly.dll");
        Assert.Throws<WeavingException>(() =>
        {
            AssemblyVersionReader.GetAssemblyVersion(path);
        });
    }
}