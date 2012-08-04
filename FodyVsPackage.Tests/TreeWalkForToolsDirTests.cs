using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class TreeWalkForToolsDirTests
{

    [Test]
    public void Existing()
    {
        Assert.IsTrue(Directory.Exists(ToolDirectoryTreeWalker.TreeWalkForToolsDirs(Environment.CurrentDirectory).First()));
    }
    [Test]
    [Ignore]//TODO: how to test
    public void NotExisting()
    {
        Assert.IsEmpty(ToolDirectoryTreeWalker.TreeWalkForToolsDirs(Path.GetTempPath()));
    }
}