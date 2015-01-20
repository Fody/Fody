using System;
using System.Collections.Generic;
using System.Linq;
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

    public static void ReadBool(this XElement config, string nodeName, Action<bool> setter)
    {
        var attribute = config.Attribute(nodeName);
        if (attribute != null)
        {
            bool value;
            if (bool.TryParse(attribute.Value, out value))
            {
                setter(value);
            }
            else
            {
                throw new WeavingException(string.Format("Could not parse '{0}' from '{1}'.", nodeName, attribute.Value));
            }
        }
    }
}