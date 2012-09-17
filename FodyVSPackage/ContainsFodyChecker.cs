using System;
using System.Linq;
using System.Xml.Linq;

public class ContainsFodyChecker
{

    public bool Check(XDocument xDocument)
    {
        try
        {
            if (xDocument.BuildDescendants("Fody.WeavingTask").Any())
            {
                return true;
            }
            return xDocument.BuildDescendants("Import")
                .Any(x =>
                         {
                             var xAttribute = x.Attribute("Project");
                             return xAttribute != null && xAttribute.Value.EndsWith("Fody.targets");
                         });
        }
        catch (Exception exception)
        {
            throw new Exception("Could not check project for weaving task.", exception);
        }
    }
}