using System;
using System.IO;
using System.Linq;

public static class PathEx
{
    public static string MakeRelativePath(string fromPath, string toPath)
    {
        if (fromPath.Last() != Path.DirectorySeparatorChar)
        {
            fromPath += Path.DirectorySeparatorChar;
        }
        if (toPath.Last() != Path.DirectorySeparatorChar)
        {
            toPath += Path.DirectorySeparatorChar;
        }
        var fromUri = new Uri(fromPath);
        var toUri = new Uri(toPath);

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
        return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }
}