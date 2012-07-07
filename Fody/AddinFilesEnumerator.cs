using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AddinFilesEnumerator
{
    public AddinDirectories AddinDirectories;

    public virtual string FindAddinAssembly(string packageName)
    {
        return GetAllAssemblyFiles(packageName)
            .OrderByDescending(AssemblyVersionReader.GetAssemblyVersion)
            .FirstOrDefault();
    }


    IEnumerable<string> GetAllAssemblyFiles(string packageName)
    {
        var packageFileName = packageName + ".dll";
        var searchPaths = AddinDirectories.SearchPaths;
        return searchPaths.SelectMany(x => Directory.EnumerateFiles(x, packageFileName, SearchOption.AllDirectories));
    }
}