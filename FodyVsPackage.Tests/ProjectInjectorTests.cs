using System.IO;
using NUnit.Framework;

[TestFixture]
public class ProjectInjectorTests
{
    [Test]
    public void WithNoWeaving()
    {
        var sourceProjectFile = Path.GetFullPath(@"TestProjects\ProjectWithNoWeaving.csproj");
        var targetFile = Path.GetTempFileName();
        File.Copy(sourceProjectFile, targetFile, true);

        try
        {
            var injector = new ProjectInjector
                               {
                                   ProjectFile = targetFile,
                               };
            injector.Execute();

            Assert.AreEqual(File.ReadAllText(Path.GetFullPath(@"TestProjects\ProjectWithWeaving.csproj")), File.ReadAllText(targetFile));
        }
        finally
        {
            File.Delete(targetFile);
        }

    }

    [Test]
    public void WithExistingWeaving()
    {
        var sourceProjectFile = Path.GetFullPath(@"TestProjects\ProjectWithWeaving.csproj");
        var targetFile = Path.GetTempFileName();
        File.Copy(sourceProjectFile, targetFile, true);

        try
        {
            var injector = new ProjectInjector
                               {
                                   ProjectFile = targetFile,
                               };
            injector.Execute();

            var source = File.ReadAllText(sourceProjectFile);
            var target = File.ReadAllText(targetFile);
            Assert.AreEqual(source, target);
        }
        finally
        {
            File.Delete(targetFile);
        }

    }

    [Test]
    public void WithOldWeaving()
    {
        var sourceProjectFile = Path.GetFullPath(@"TestProjects\ProjectWithOldWeaving.csproj");
        var targetFile = Path.GetTempFileName();
        File.Copy(sourceProjectFile, targetFile, true);

        try
        {
            var injector = new ProjectInjector
                               {
                                   ProjectFile = targetFile,
                               };
            injector.Execute();

            Assert.AreEqual(File.ReadAllText(Path.GetFullPath(@"TestProjects\ProjectWithWeaving.csproj")), File.ReadAllText(targetFile));
        }
        finally
        {
            File.Delete(targetFile);
        }

    }

}

