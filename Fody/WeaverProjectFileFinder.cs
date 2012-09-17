using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Processor
{
    public string WeaverAssemblyPath;
    public bool FoundWeaverProjectFile;

    public void FindWeaverProjectFile()
    {
        WeaverAssemblyPath = GetAllAssemblyFiles()
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();
        if (WeaverAssemblyPath == null)
        {
            Logger.LogInfo("No Weaver project file found.");
            FoundWeaverProjectFile = false;
        }
        else
        {
            Logger.LogInfo(string.Format("Weaver project file found at '{0}'.", WeaverAssemblyPath));
            FoundWeaverProjectFile = true;
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