using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    public List<string> FodyFiles = new List<string>();


    public string FindAddinAssembly(string packageName)
    {
        var packageFileName = packageName + ".Fody.dll";
        return FodyFiles.Where(x => string.Equals(Path.GetFileName(x), packageFileName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(AssemblyVersionReader.GetAssemblyVersion)
            .FirstOrDefault();
    }
}