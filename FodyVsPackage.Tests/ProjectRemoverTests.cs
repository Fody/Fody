using System.IO;
using NUnit.Framework;

[TestFixture]
public class ProjectRemoverTests
{
    [Test]
    public void WithNoWeavingNotChanged()
    {
        var sourceProjectFile = Path.GetFullPath(@"TestProjects\ProjectWithNoWeaving.csproj");
        var targetFile = Path.GetTempFileName();
        File.Copy(sourceProjectFile, targetFile, true);
        try
        {
            new ProjectRemover(targetFile);
            Assert.AreEqual(File.ReadAllText(Path.GetFullPath(@"TestProjects\ProjectWithNoWeaving.csproj")), File.ReadAllText(targetFile));
        }
        finally
        {
            File.Delete(targetFile);
        }
    }


    [Test]
    public void WeavingRemoved()
    {
        var sourceProjectFile = Path.GetFullPath(@"TestProjects\ProjectWithWeaving.csproj");
        var targetFile = Path.GetTempFileName();
        File.Copy(sourceProjectFile, targetFile, true);
        try
        {

            new ProjectRemover(targetFile);
            Assert.AreEqual(File.ReadAllText(Path.GetFullPath(@"TestProjects\ProjectWithNoWeaving.csproj")), File.ReadAllText(targetFile));
    
        }
        finally
        {
            File.Delete(targetFile);
        }
    }

    [Test]
    public void OldWeavingRemoved()
    {
        var sourceProjectFile = Path.GetFullPath(@"TestProjects\ProjectWithOldWeaving.csproj");
        var targetFile = Path.GetTempFileName();
        File.Copy(sourceProjectFile, targetFile, true);
        try
        {

            new ProjectRemover(targetFile);
            Assert.AreEqual(File.ReadAllText(Path.GetFullPath(@"TestProjects\ProjectWithNoWeaving.csproj")), File.ReadAllText(targetFile));
    
        }
        finally
        {
            File.Delete(targetFile);
        }
    }
}