using System.IO;
using NUnit.Framework;


[TestFixture]
public class WeaverTypeLoaderTests
{
    [Test]
    public void Simple()
    {
        var taskTypeLoader = new ModuleWeaverTypeLoader(new PackagePathFinder
                                                    {
                                                        PackagesPath = Path.GetFullPath("Packages")
                                                    });
        taskTypeLoader.LoadInstance("SampleTask");
    }
}