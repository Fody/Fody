using System;
using System.IO;
using System.Linq;

public partial class Processor
{
    public string WeaverAssemblyPath;
    public bool FoundWeaverProjectFile;

    public void FindWeaverProjectFile()
    {
        GetValue();
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

    void GetValue()
    {
        var weaversBin = Path.Combine(SolutionDirectoryPath, @"Weavers\bin");
        if (Directory.Exists(weaversBin))
        {
            WeaverAssemblyPath = Directory.EnumerateFiles(weaversBin, "Weavers.dll", SearchOption.AllDirectories)
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();
            return;
        }

        //Hack for ncrunch
        //<Reference Include="...\AppData\Local\NCrunch\2544\1\Integration\Weavers\bin\Debug\Weavers.dll" />
        WeaverAssemblyPath = References.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(x => x.EndsWith("Weavers.dll"));
    }

}