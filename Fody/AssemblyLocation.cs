using System.IO;

public static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;

        var path = assembly.Location
            .Replace("file:///", "")
            .Replace("file://", "")
            .Replace(@"file:\\\", "")
            .Replace(@"file:\\", "");

        CurrentDirectory = Path.GetDirectoryName(path);
    }

    public static readonly string CurrentDirectory;
}