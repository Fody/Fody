using System;
using System.IO;
using Fody;

public class ConfigDirectoryFinder
{
    public WeavingTask WeavingTask;
    public ILogger Logger;
    public AddinDirectories AddinDirectories;

    public void Execute()
    {
        var addinSearchPaths = WeavingTask.AddinSearchPaths;

        if (addinSearchPaths != null)
        {
            foreach (var addinSearchPath in addinSearchPaths.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var fullPath = Path.GetFullPath(Path.Combine(WeavingTask.SolutionDir, addinSearchPath));
                if (Directory.Exists(fullPath))
                {
                    AddinDirectories.SearchPaths.Add(fullPath);
                }
                else
                {
                    Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", fullPath));
                }
            }
        }

    }

}