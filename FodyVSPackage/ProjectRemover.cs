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
        RemoveImport();
        RemoveFodyWeaversXmlContent();
        xDocument.Save(projectFile);
        DeleteFodyWeaversXmlFile(projectFile);
    }

    void RemoveFodyWeaversXmlContent()
    {
        xDocument.Descendants()
            .Where(x => (string)x.Attribute("Include")== "FodyWeavers.xml")
            .Remove();
    }

    void DeleteFodyWeaversXmlFile(string projectFile)
    {
        var tasksPath = Path.Combine(Path.GetDirectoryName(projectFile), "FodyWeavers.xml");
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
                           return xAttribute != null && xAttribute.Value.EndsWith("Fody.targets");
                       })
            .Remove();
    }
}