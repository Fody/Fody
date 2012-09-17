using System;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AddNugetDirectoryToAddinSearchTests
{
    [Test]
    public void Simple()
    {
        var logger = new Mock<BuildLogger>().Object;
        var processor = new AddinFinder
                           {
                               Logger = logger,
                               PackagesPath = Environment.CurrentDirectory
                           };
        processor.AddNugetDirectoryToAddinSearch();
        var searchPaths = processor.AddinSearchPaths;
        Assert.AreEqual(Environment.CurrentDirectory, searchPaths[0]);
    }

}