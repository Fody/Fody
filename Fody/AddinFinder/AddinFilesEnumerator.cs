using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    private List<string> fodyFiles;

    public List<string> FodyFiles
    {
        get
        {
            if (fodyFiles == null)
            {
                fodyFiles = new List<string>();
                FindAddinDirectoriesLegacy();
            }

            return fodyFiles;
        }
    }

    public string FindAddinAssembly(string packageName)
    {
        if (weaversFromWellKnownPaths == null)
            throw new InvalidOperationException("you must call FindAddinDirectories() first");

        if (weaversFromWellKnownPaths.TryGetValue(packageName, out var filePath))
            return filePath;

        var packageFileName = packageName + ".Fody.dll";
        return FodyFiles.Where(x => string.Equals(Path.GetFileName(x), packageFileName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(AssemblyVersionReader.GetAssemblyVersion)
            .FirstOrDefault();
    }
}