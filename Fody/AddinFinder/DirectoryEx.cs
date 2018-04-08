using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class DirectoryEx
{
    public static IEnumerable<string> EnumerateFilesEndsWith(string directory, string end, SearchOption searchOption)
    {
        return Directory.EnumerateFiles(directory, "*", searchOption)
            .Where(_ => _.EndsWith(end, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<string> EnumerateDirectoriesEndsWith(string directory, string end)
    {
        return Directory.EnumerateDirectories(directory, "*")
            .Where(_ => _.EndsWith(end, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<string> EnumerateDirectoriesContains(string directory, string contains)
    {
        return Directory.EnumerateDirectories(directory)
            .Where(_ => _.Contains(contains, StringComparison.OrdinalIgnoreCase));
    }

    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source?.IndexOf(toCheck, comp) >= 0;
    }
}