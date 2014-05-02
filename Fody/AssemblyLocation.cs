using System;
using System.IO;

public static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;
        if (assembly.Location.Contains("#"))
        {
            throw new NotSupportedException("'#' character in path is not supported while building projects containing Fody.");
        }

        var path = assembly.Location
            .Replace("file:///", "")
            .Replace("file://", "")
            .Replace(@"file:\\\", "")
            .Replace(@"file:\\", "");

        CurrentDirectory = Path.GetDirectoryName(path);
    }

    public static string CurrentDirectory;
}