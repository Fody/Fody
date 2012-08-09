using System.Collections.Generic;
using System.IO;
using NUnit.Framework;


[TestFixture]
public class AddinFilesEnumeratorTests
{
    [Test]
    public void NotFound()
    {
        var processor = new Processor
            {
                AddinSearchPaths = new List<string>
                    {
                        Path.GetFullPath("Packages")
                    }
            };
        processor.CacheAllFodyAddinDlls();
        Assert.IsNull(processor.FindAddinAssembly("DoesNotExist"));
    }

    [Test]
    public void Valid()
    {
        var processor = new Processor
            {
                AddinSearchPaths = new List<string>
                    {
                        Path.GetFullPath("Packages")
                    }
            };
        processor.CacheAllFodyAddinDlls();
        Assert.IsNotNull( processor.FindAddinAssembly("SampleTask"));
    }
}