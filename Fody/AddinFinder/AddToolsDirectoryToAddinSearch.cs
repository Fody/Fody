using System.IO;

public partial class AddinFinder
{

    public void AddToolsSolutionDirectoryToAddinSearch()
    {
        var solutionDirToolsDirectory = Path.GetFullPath(Path.Combine(SolutionDir, "Tools"));
        if (Directory.Exists(solutionDirToolsDirectory))
        {
            AddinSearchPaths.Add(solutionDirToolsDirectory);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", solutionDirToolsDirectory));
        }
    }
}