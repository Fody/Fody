using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ProjectInjector
{
    public string ProjectFile;
    XDocument xDocument;

    public void Execute()
    {
        new FileInfo(ProjectFile).IsReadOnly = false;
        xDocument = XDocument.Load(ProjectFile);
        OldProjectRemover.Remove(xDocument);
        InjectImport();
        InjectWeaversContent();
        xDocument.Save(ProjectFile);
    }

    void InjectWeaversContent()
    {
        var exists = xDocument.Descendants()
            .Any(x =>
                     {
                         var xAttribute = x.Attribute("Include");
                         return xAttribute != null && xAttribute.Value == "FodyWeavers.xml";
                     });
        if (exists)
        {
            return;
        }
    
        var itemGroup = xDocument.BuildDescendants("ItemGroup").FirstOrDefault();
        if (itemGroup == null)
        {
            itemGroup = new XElement(MsBuildXmlExtensions.BuildNamespace + "ItemGroup");
            xDocument.Root.Add(itemGroup);
        }
        itemGroup.Add(new XElement(MsBuildXmlExtensions.BuildNamespace + "None", new XAttribute("Include", "FodyWeavers.xml")));
    }

    void InjectImport()
    {
        // <Import Project="$(SolutionDir)\Tools\Fody\Fody.targets" />
        var exists = xDocument.BuildDescendants("Import").Any(x =>
                                                                  {
                                                                      var xAttribute = x.Attribute("Project");
                                                                      return xAttribute != null && xAttribute.Value.EndsWith("Fody.targets",StringComparison.InvariantCultureIgnoreCase);
                                                                  });
        if (exists)
        {
            return;
        }
        xDocument.Root.Add(new XElement(MsBuildXmlExtensions.BuildNamespace + "Import", new XAttribute("Project", @"$(SolutionDir)\Tools\Fody\Fody.targets")));

    }
}