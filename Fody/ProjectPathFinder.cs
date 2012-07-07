using System.IO;
using Fody;

public class ProjectPathFinder
{
    public WeavingTask WeavingTask;
    public BuildLogger Logger;
    public string ProjectFilePath;

    public virtual void Execute()
    {
        ProjectFilePath = GetProjectPath();
        Logger.LogInfo(string.Format("ProjectFilePath path is '{0}.'", ProjectFilePath));
    }

    string GetProjectPath()
    {
        if (!File.Exists(WeavingTask.ProjectPath))
        {
            throw new WeavingException(string.Format("ProjectPath \"{0}\" does not exist.", WeavingTask.ProjectPath));
        }
        return WeavingTask.ProjectPath;
    }
}