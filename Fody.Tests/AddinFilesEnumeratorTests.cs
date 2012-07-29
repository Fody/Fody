using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class AddinFilesEnumeratorTests
{
    [Test]
    public void NotFound()
    {
        var taskTypeLoader = new AddinFilesEnumerator
            {
                AddinDirectories = new List<string>
                    {
                        Path.GetFullPath("Packages")
                    }
            };
        Assert.IsNull(taskTypeLoader.FindAddinAssembly("DoesNotExist"));
    }

    [Test]
    public void Valid()
    {
        var taskTypeLoader = new AddinFilesEnumerator
            {
                AddinDirectories = new List<string>
                    {
                        Path.GetFullPath("Packages")
                    }
            };
        taskTypeLoader.FindAddinAssembly("SampleTask.Fody");
    }
}