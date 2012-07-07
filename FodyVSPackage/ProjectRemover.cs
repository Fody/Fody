using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ProjectRemover
{
    XDocument xDocument;

    public ProjectRemover(string projectFile)
    {
        new FileInfo(projectFile).IsReadOnly = false;
        xDocument = XDocument.Load(projectFile);
        OldProjectRemover.Remove(xDocument);
        RemoveImport();
        RemoveFodyWeaversXmlContent();
        xDocument.Save(projectFile);
        DeleteFodyWeaversXmlFile(projectFile);
    }

    void RemoveFodyWeaversXmlContent()
    {
        xDocument.Descendants()
            .Where(x => string.Equals((string)x.Attribute("Include"), ConfigFile.FodyWeaversXml, StringComparison.InvariantCultureIgnoreCase))
            .Remove();
    }

    void DeleteFodyWeaversXmlFile(string projectFile)
    {
        var tasksPath = Path.Combine(Path.GetDirectoryName(projectFile), ConfigFile.FodyWeaversXml);
        if (File.Exists(tasksPath))
        {
            new FileInfo(tasksPath).IsReadOnly = false;
            File.Delete(tasksPath);
        }
    }

    void RemoveImport()
    {
        xDocument.BuildDescendants("Import")
            .Where(x =>
                       {
                           var xAttribute = x.Attribute("Project");
                           return xAttribute != null && xAttribute.Value.EndsWith("Fody.targets", StringComparison.InvariantCultureIgnoreCase);
                       })
            .Remove();
    }
}
public static class OldProjectRemover{

//TODO: remove
    public static void Remove(XDocument xDocument)
    {
        xDocument.BuildDescendants("Target")
            .Where(x => string.Equals((string)x.Attribute("Name"), "AfterCompile", StringComparison.InvariantCultureIgnoreCase))
            .Descendants(MsBuildXmlExtensions.BuildNamespace + "Fody.WeavingTask")
            .Remove();
        xDocument.BuildDescendants("UsingTask")
            .Where(x => (string)x.Attribute("TaskName") == "Fody.WeavingTask")
            .Remove();
    }

}