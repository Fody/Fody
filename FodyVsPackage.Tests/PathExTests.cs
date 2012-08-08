using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class PathExTests
{

    [Test]
    public void MakeRelativePath()
    {
        var parent = Directory.GetParent(Environment.CurrentDirectory).FullName;
        var makeRelativePath = PathEx.MakeRelativePath(parent, Environment.CurrentDirectory );
        var expected = Environment.CurrentDirectory.Split(Path.DirectorySeparatorChar).Last() + Path.DirectorySeparatorChar;
        Assert.AreEqual(expected,makeRelativePath);
    }
    [Test]
    public void MakeRelativePathHigher()
    {
        var parent = Directory.GetParent(Environment.CurrentDirectory).FullName;
        var makeRelativePath = PathEx.MakeRelativePath(Environment.CurrentDirectory , parent );
        Assert.AreEqual(@"..\",makeRelativePath);
    }
}