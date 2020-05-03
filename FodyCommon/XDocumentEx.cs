using System.IO;
using System.Xml;
using System.Xml.Linq;

public static class XDocumentEx
{
    public static XDocument Load(string path)
    {
        try
        {
            using var reader = new StreamReader(FileEx.OpenRead(path));
            return XDocument.Load(reader);
        }
        catch (XmlException exception)
        {
            throw new WeavingException($"Could not read '{path}' because it has invalid XML. Message: '{exception.Message}'.");
        }
    }

    public static XDocument Parse(string text)
    {
        try
        {
            return XDocument.Parse(text);
        }
        catch (XmlException exception)
        {
            throw new WeavingException($"Could not parse XML. Message: '{exception.Message}'.");
        }
    }
}