using System;
using System.Collections.Generic;
using System.IO.Abstractions;

public static class WeaversHistory
{
    public static Dictionary<string, DateTime> TimeStamps = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

    public static bool HasChanged(IEnumerable<string> weaverPaths, IFileSystem fileSystem = null)
    {
        if (fileSystem == null)
            fileSystem = new FileSystem();

        var changed = false;
        foreach (var weaverPath in weaverPaths)
        {
            var newVersion = fileSystem.File.GetLastWriteTimeUtc(weaverPath);
            DateTime dateTime;
            if (TimeStamps.TryGetValue(weaverPath, out dateTime))
            {
                if (dateTime != newVersion)
                {
                    changed = true;
                }
            }
            TimeStamps[weaverPath] = newVersion;
        }
        return changed;
    }
}