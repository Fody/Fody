using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Fody;

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
        if (attribute != null)
        {
            try
            {
                value = XmlConvert.ToBoolean(attribute.Value);
                return true;
            }
            catch (Exception ex)
            {
                throw new WeavingException($"Could not parse '{nodeName}' from '{attribute.Value}'.");
            }
        }
        value = false;
        return false;
    }
}