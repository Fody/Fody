using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class AddinFinder
{
    public List<string> FodyFiles = new List<string>();

    public Func<string, Version> VersionReader = AssemblyVersionReader.GetAssemblyVersion;

    public string FindAddinAssembly(string packageName)
    {
        var packageFileName = packageName + ".Fody.dll";

        var candidates = FodyFiles.Where(x => string.Equals(Path.GetFileName(x), packageFileName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(ProbingPathScore)
            .ThenByDescending(VersionReader);

        return candidates
            .FirstOrDefault();
    }

    private int ProbingPathScore(string filePath)
    {
        var score = weaverProbingPaths.Any(probingPath => filePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).StartsWith(probingPath, StringComparison.OrdinalIgnoreCase)) ? 0 : 1;

        return score;
    }
}