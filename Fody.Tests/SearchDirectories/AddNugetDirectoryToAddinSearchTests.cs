using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class AddNugetDirectoryToAddinSearchTests
{
    [Test]
    public void Simple()
    {
        var processor = new AddinFinder
                           {
                               SolutionDirectoryPath = Environment.CurrentDirectory
                           };
        processor.AddNugetDirectoryToAddinSearch();
        var searchPaths = processor.AddinSearchPaths;
        var expected = Path.Combine(Environment.CurrentDirectory, "Packages");
        Assert.AreEqual(expected , searchPaths[0]);
    }

}