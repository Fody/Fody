using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class FodyDirectoryFinderTests
{

    [Test]
    public void Existing()
    {
        var fodyDir = FodyDirectoryFinder.TreeWalkForToolsFodyDir(AssemblyLocation.CurrentDirectory());
        Assert.IsTrue(Directory.Exists(fodyDir));
    }

    [Test]
    public void NotExisting()
    {
        Assert.IsNull(FodyDirectoryFinder.TreeWalkForToolsFodyDir(Path.GetTempPath()));
    }
}