using System.Xml.Linq;

public class WeaverConfigFile
{
    public bool IsGlobal;
    public readonly string FilePath;
    public readonly XDocument Document;

    public WeaverConfigFile(string filePath)
    {
        FilePath = filePath;
        Document = XDocumentEx.Load(FilePath);
    }
}