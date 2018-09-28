using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public static class AddinProbingPathFinder
{
    public static IDictionary<string, string> Lookup(string weaverProbingPaths, Action<string> log)
    {
        var probingPaths = weaverProbingPaths?.Split(';')
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => item.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            .Select(Path.GetDirectoryName) // .props file is in the build sub-directory => package root is the parent folder.
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => item.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
            .OrderByDescending(item => item.Length)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var waverFiles = probingPaths?
            .Select(path => GetAssemblyFromNugetDir(path, log))
            .Where(path => !string.IsNullOrEmpty(path))
            .ToArray();

        return waverFiles?.ToDictionary(GetAddinNameFromWeaverFile, StringComparer.OrdinalIgnoreCase) 
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    static string GetAddinNameFromWeaverFile(string filePath)
    {
        Debug.Assert(filePath.EndsWith(".Fody.dll", StringComparison.OrdinalIgnoreCase));

        // remove .Fody.dll
        return Path.ChangeExtension(Path.GetFileNameWithoutExtension(filePath), null);
    }

    static string GetAssemblyFromNugetDir(string nugetDir, Action<string> log)
    {
#if (NETSTANDARD2_0)
        var specificDir = Path.Combine(nugetDir, "netstandardweaver");
#endif
#if (NET46)
        var specificDir = Path.Combine(nugetDir, "netclassicweaver");
#endif

        var weaverAssembly = GetAssemblyFromDir(specificDir) ?? GetAssemblyFromDir(nugetDir);

        if (weaverAssembly == null)
        {
            log($"Invalid weaver probing path: No weaver found at {nugetDir}");
        }

        return weaverAssembly;
    }

    static string GetAssemblyFromDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return null;
        }

        return Directory.GetFiles(dir)
            .FirstOrDefault(x => Path.GetFileName(x).EndsWith(@".Fody.dll", StringComparison.OrdinalIgnoreCase));
    }
}
