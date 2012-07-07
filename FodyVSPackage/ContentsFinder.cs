using System.IO;

public class ContentsFinder
{

    public string ContentFilesPath;

    public ContentsFinder()
    {
        var directoryName = Path.GetDirectoryName(GetType().Assembly.Location);
        ContentFilesPath = Path.Combine(directoryName, "ContentFiles");
    }
}