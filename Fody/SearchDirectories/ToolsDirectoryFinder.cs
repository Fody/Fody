using System.IO;
using Fody;

public class ToolsDirectoryFinder
{
    public WeavingTask WeavingTask;
    public ILogger Logger;
    public AddinDirectories AddinDirectories;

    public void Execute()
    {
        var toolsDirectory = Path.GetFullPath(Path.Combine(WeavingTask.SolutionDir, "Tools"));
        if (Directory.Exists(toolsDirectory))
        {
            AddinDirectories.SearchPaths.Add(toolsDirectory);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", toolsDirectory));
        }
    }
}