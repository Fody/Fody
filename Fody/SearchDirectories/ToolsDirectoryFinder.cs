using System.IO;

public partial class Processor
{

    public void AddToolsDirectoryToAddinSearch()
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
        var parent = Directory.GetParent(AssemblyLocation.CurrentDirectory()).FullName;
        var assemblyLocationToolsDirectory = Path.GetFullPath(Path.Combine(parent, "Tools"));
        
        if (assemblyLocationToolsDirectory == solutionDirToolsDirectory)
        {
            return;
        }
        if (Directory.Exists(assemblyLocationToolsDirectory))
        {
            AddinSearchPaths.Add(assemblyLocationToolsDirectory);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", assemblyLocationToolsDirectory));
        }
    }
}