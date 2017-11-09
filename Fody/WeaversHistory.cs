using System;
using System.Collections.Generic;
using System.IO;

public static class WeaversHistory
{
    public static Dictionary<string, DateTime> TimeStamps = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

    public static bool HasChanged(IEnumerable<string> weaverPaths)
    {
        var changed = false;
        foreach (var weaverPath in weaverPaths)
        {
            var newVersion = File.GetLastWriteTimeUtc(weaverPath);
            if (TimeStamps.TryGetValue(weaverPath, out var dateTime))
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