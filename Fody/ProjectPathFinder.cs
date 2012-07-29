using System.IO;

public class ProjectPathValidator
{
    public string ProjectPath;
    public BuildLogger Logger;

    public virtual void Execute()
    {
        if (!File.Exists(ProjectPath))
        {
            throw new WeavingException(string.Format("ProjectPath \"{0}\" does not exist.", ProjectPath));
        }
        Logger.LogInfo(string.Format("ProjectFilePath path is '{0}.'", ProjectPath));
    }

}