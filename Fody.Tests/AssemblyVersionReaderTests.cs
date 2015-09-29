using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class AssemblyVersionReaderTests
{
    [Test]
    public void BadImage()
    {
        Assert.Throws<WeavingException>(() => AssemblyVersionReader.GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, "BadAssembly.dll")));
    }
}