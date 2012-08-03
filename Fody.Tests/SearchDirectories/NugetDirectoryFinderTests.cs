using System;
using Moq;
using NUnit.Framework;

[TestFixture]
public class NugetDirectoryFinderTests
{
    [Test]
    public void Simple()
    {
        var logger = new Mock<BuildLogger>().Object;
        var searcher = new Processor
                           {
                               Logger = logger,
                               PackagesPath = Environment.CurrentDirectory
                           };
        searcher.AddNugetDirectoryToAddinSearch();
        var searchPaths = searcher.AddinSearchPaths;
        Assert.AreEqual(Environment.CurrentDirectory, searchPaths[0]);
    }

}