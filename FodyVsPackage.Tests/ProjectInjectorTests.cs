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
                                   FodyToolsDirectory = @"$(SolutionDir)\Tools\Fody\"
                               };
            injector.Execute();

            Assert.AreEqual(FileReader.Read(@"TestProjects\ProjectWithWeaving.csproj"), FileReader.Read(targetFile));
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

            var source = FileReader.Read(sourceProjectFile);
            var target = FileReader.Read(targetFile);
            Assert.AreEqual(source, target);
        }
        finally
        {
            File.Delete(targetFile);
        }

    }


}