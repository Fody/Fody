#if(DEBUG)
using System;
using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class NugetPackagePathFinderTest
{
    [Test]
    public void NoNugetConfig()
    {
        var processor = new AddinFinder
            {
                SolutionDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../NugetPackagePathFinder/FakeSolution")),
                Logger = new Mock<BuildLogger>().Object
            };

        processor.FindNugetPackagePath();
        Assert.IsTrue(processor.PackagesPath.EndsWith("\\FakeSolution\\Packages"));
    }

    
}
#endif