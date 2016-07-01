using System;
using System.IO;
using System.Linq;

public partial class Processor
{
    public string WeaverAssemblyPath;
    public bool FoundWeaverProjectFile;

    public virtual void FindWeaverProjectFile()
    {
        GetValue();
        if (WeaverAssemblyPath == null)
        {
            Logger.LogDebug("No Weaver project file found.");
            FoundWeaverProjectFile = false;
        }
        else
        {
            Logger.LogDebug($"Weaver project file found at '{WeaverAssemblyPath}'.");
            FoundWeaverProjectFile = true;
        }
    }

    void GetValue()
    {
        var weaversBin = FileSystem.Path.Combine(SolutionDirectory, "Weavers", "bin");

        if (FileSystem.Directory.Exists(weaversBin))
        {
            WeaverAssemblyPath = FileSystem.Directory.EnumerateFiles(weaversBin, "Weavers.dll", SearchOption.AllDirectories)
                .OrderByDescending(FileSystem.File.GetLastWriteTime)
                .FirstOrDefault();
            return;
        }

        //Hack for ncrunch
        //<Reference Include="...\AppData\Local\NCrunch\2544\1\Integration\Weavers\bin\Debug\Weavers.dll" />
        WeaverAssemblyPath = References.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(x => x.EndsWith("Weavers.dll"));
    }

}