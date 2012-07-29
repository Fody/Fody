using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WeaverProjectFileFinder
{
    public string SolutionDir;
    public BuildLogger Logger;
    public string WeaverAssemblyPath;
    public bool Found;

    public virtual void Execute()
    {
        WeaverAssemblyPath = GetAllAssemblyFiles()
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();
        if (WeaverAssemblyPath == null)
        {
            Logger.LogInfo("No Weaver project file found.");
            Found = false;
        }
        else
        {
            Logger.LogInfo(string.Format("Weaver project file found at '{0}'.", WeaverAssemblyPath));
            Found = true;
        }
    }

    IEnumerable<string> GetAllAssemblyFiles()
    {
        var weaversBin = Path.Combine(SolutionDir, @"Weavers\bin");
        if (Directory.Exists(weaversBin))
        {
            return Directory.EnumerateFiles(weaversBin, "Weavers.dll", SearchOption.AllDirectories);
        }
        return Enumerable.Empty<string>();
    }
}