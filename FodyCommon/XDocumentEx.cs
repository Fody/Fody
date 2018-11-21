using System.IO;
using System.Xml.Linq;

public static class XDocumentEx
{
    public static XDocument Load(string path)
    {
        using (var stream = FileEx.OpenRead(path))
        using (var reader = new StreamReader(stream))
        {
            return XDocument.Load(reader);
        }
    }
}