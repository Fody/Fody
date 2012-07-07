using System.IO;

public static class FileReader
{
    
    public static string Read(string path)
    {
        return File.ReadAllText(Path.GetFullPath(path))
            .Replace(@"\n", @"\r\n");
    }
}