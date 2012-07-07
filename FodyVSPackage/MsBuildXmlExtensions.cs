using System.Collections.Generic;
using System.Xml.Linq;

public static class MsBuildXmlExtensions
{
    public const string BuildNamespace = "{http://schemas.microsoft.com/developer/msbuild/2003}";

    public static IEnumerable<XElement> BuildDescendants(this XContainer document, XName name)
    {
        return document.Descendants(BuildNamespace + name);
    }
}