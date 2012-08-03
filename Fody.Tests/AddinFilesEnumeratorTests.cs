using System.Collections.Generic;
using System.IO;
using NUnit.Framework;


[TestFixture]
public class AddinFilesEnumeratorTests
{
    [Test]
    public void NotFound()
    {
        var taskTypeLoader = new Processor
                                 {
                AddinSearchPaths = new List<string>
                    {
                        Path.GetFullPath("Packages")
                    }
            };
        taskTypeLoader.CacheAllFodyAddinDlls();
        Assert.IsNull(taskTypeLoader.FindAddinAssembly("DoesNotExist"));
    }

    [Test]
    public void Valid()
    {
        var taskTypeLoader = new Processor
                                 {
            AddinSearchPaths = new List<string>
                    {
                        Path.GetFullPath("Packages")
                    }
            };
        taskTypeLoader.CacheAllFodyAddinDlls();
        taskTypeLoader.FindAddinAssembly("SampleTask.Fody");
    }
}