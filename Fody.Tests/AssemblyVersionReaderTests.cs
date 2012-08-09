using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class AssemblyVersionReaderTests
{
    [Test]
    [ExpectedException(typeof(WeavingException))]
    public void BadImage()
    {
        AssemblyVersionReader.GetAssemblyVersion( Path.Combine(Environment.CurrentDirectory, "BadAssembly.dll"));
    }
}