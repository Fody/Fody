using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Processor
{
    List<string> fodyFiles;

    public void CacheAllFodyAddinDlls()
    {
        fodyFiles = AddinSearchPaths.SelectMany(x => Directory.EnumerateFiles(x, "*.Fody.dll", SearchOption.AllDirectories))
            .ToList();
    }
    public virtual string FindAddinAssembly(string packageName)
    {
        return GetAllAssemblyFiles(packageName)
            .OrderByDescending(AssemblyVersionReader.GetAssemblyVersion)
            .FirstOrDefault();
    }


    IEnumerable<string> GetAllAssemblyFiles(string packageName)
    {
        var packageFileName = packageName + ".dll";
        return fodyFiles.Where(x => string.Equals(x, packageFileName,StringComparison.OrdinalIgnoreCase));
    }
}