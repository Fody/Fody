using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    List<string> fodyFiles;


    public void CacheAllFodyAddinDlls()
    {
        fodyFiles = AddinSearchPaths
            .SelectMany(x => Directory.EnumerateFiles(x, "*.Fody.dll", SearchOption.AllDirectories))
            .ToList();
    }

    public string FindAddinAssembly(string packageName)
    {
        var packageFileName = packageName + ".Fody.dll";
        return fodyFiles.Where(x => x.EndsWith(packageFileName))
            .OrderByDescending(AssemblyVersionReader.GetAssemblyVersion)
            .FirstOrDefault();
    }
}