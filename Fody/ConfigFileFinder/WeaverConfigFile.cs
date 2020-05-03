using System.Xml.Linq;

public class WeaverConfigFile
{
    public readonly bool AllowExtraEntries;
    public readonly string? FilePath;
    public readonly XDocument Document;

    public WeaverConfigFile(string filePath, bool allowExtraEntries = false)
    {
        AllowExtraEntries = allowExtraEntries;
        FilePath = filePath;
        Document = XDocumentEx.Load(FilePath);
    }

    public WeaverConfigFile(XDocument document)
    {
        AllowExtraEntries = true;
        Document = document;
    }
}