using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

public static class XmlExtensions
{
    public static void StripNamespace(this XDocument document)
    {
        if (document.Root == null)
        {
            return;
        }
        foreach (var element in document.Root.DescendantsAndSelf())
        {
            element.Name = element.Name.LocalName;
            element.ReplaceAttributes(GetAttributes(element).ToList());
        }
    }

    static IEnumerable<XAttribute> GetAttributes(XElement xElement)
    {
        return xElement.Attributes()
            .Where(x => !x.IsNamespaceDeclaration)
            .Select(x => new XAttribute(x.Name.LocalName, x.Value));
    }

    public static bool TryReadBool(this XElement config, string nodeName, out bool value)
    {
        var attribute = config.Attribute(nodeName);
        if (attribute == null)
        {
            value = false;
            return false;
        }

        try
        {
            value = XmlConvert.ToBoolean(attribute.Value.ToLowerInvariant());
            return true;
        }
        catch
        {
            throw new WeavingException($"Could not parse '{nodeName}' from '{attribute.Value}'.");
        }
    }
}