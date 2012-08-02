using System.IO;

public class ToolsDirectoryFinder
{
    public string SolutionDir;
    public ILogger Logger;
    public AddinDirectories AddinDirectories;

    public void Execute()
    {
        var solutionDirToolsDirectory = Path.GetFullPath(Path.Combine(SolutionDir, "Tools"));
        if (Directory.Exists(solutionDirToolsDirectory))
        {
            AddinDirectories.SearchPaths.Add(solutionDirToolsDirectory);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", solutionDirToolsDirectory));
        }
        var assemblyLocationToolsDirectory = Path.GetFullPath(Path.Combine(Directory.GetParent(AssemblyLocation.CurrentDirectory()).FullName, "Tools"));
        
        if (assemblyLocationToolsDirectory == solutionDirToolsDirectory)
        {
            return;
        }
        if (Directory.Exists(assemblyLocationToolsDirectory))
        {
            AddinDirectories.SearchPaths.Add(assemblyLocationToolsDirectory);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", assemblyLocationToolsDirectory));
        }
    }
}