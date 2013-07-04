using System.Collections.Generic;
using System.Linq;

public partial class AddinFinder
{
    public List<string> FodyFiles = new List<string>();


    public string FindAddinAssembly(string packageName)
    {
        var packageFileName = packageName + ".Fody.dll";
        return FodyFiles.Where(x => x == packageFileName)
            .OrderByDescending(AssemblyVersionReader.GetAssemblyVersion)
            .FirstOrDefault();
    }
}