using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fody;

public partial class AddinFinder
{
    public List<string> FodyFiles = new List<string>();

    public string FindAddinAssembly(string packageName, VersionFilter versionFilter)
    {
        var packageFileName = packageName + ".Fody.dll";
        return (from file in FodyFiles
                where string.Equals(Path.GetFileName(file), packageFileName, StringComparison.OrdinalIgnoreCase)
                let version = AssemblyVersionReader.GetAssemblyVersion(file)
                where versionFilter == null || versionFilter.IsMatch(version)
                orderby version descending
                select file).FirstOrDefault();
    }
}