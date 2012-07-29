using System;
using System.IO;

public class ConfigDirectoryFinder
{
    public string SolutionDir;
    public string AddinSearchPaths;
    public ILogger Logger;
    public AddinDirectories AddinDirectories;

    public void Execute()
    {
     
        if (AddinSearchPaths != null)
        {
            foreach (var addinSearchPath in AddinSearchPaths.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var fullPath = Path.GetFullPath(Path.Combine(SolutionDir, addinSearchPath));
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