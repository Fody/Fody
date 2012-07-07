using System;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class NugetDirectoryFinderTests
{
    [Test]
    public void Simple()
    {
        var logger = Substitute.For<ILogger>();
        var nugetPackagePathFinder = Substitute.For<NugetPackagePathFinder>();
        nugetPackagePathFinder.PackagesPath = Environment.CurrentDirectory;
        var searchDirectories = new AddinDirectories
                                    {
                                        Logger = logger
                                    };
        var searcher = new NugetDirectoryFinder
                           {
                               Logger = logger,
                               AddinDirectories = searchDirectories,
                               NugetPackagePathFinder = nugetPackagePathFinder
                           };
        searcher.Execute();
        var searchPaths = searchDirectories.SearchPaths;
        Assert.AreEqual(Environment.CurrentDirectory, searchPaths[0]);
    }

}