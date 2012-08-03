using System;
using System.IO;

public partial class Processor
{

    public void AddMsBuildConfigToAddinSearch()
    {
     
        if (AddinSearchPathsFromMsBuild != null)
        {
            foreach (var addinSearchPath in AddinSearchPathsFromMsBuild.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var fullPath = Path.GetFullPath(Path.Combine(SolutionDir, addinSearchPath));
                if (Directory.Exists(fullPath))
                {
                    AddinSearchPaths.Add(fullPath);
                }
                else
                {
                    Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", fullPath));
                }
            }
        }

    }

}