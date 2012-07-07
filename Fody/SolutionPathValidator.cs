using System.IO;
using Fody;

public class SolutionPathValidator
{
    public BuildLogger Logger;
    public WeavingTask WeavingTask;

    public void Execute()
    {
        if (!Directory.Exists(WeavingTask.SolutionDir))
        {
            throw new WeavingException(string.Format("SolutionDir \"{0}\" does not exist.", WeavingTask.SolutionDir));
        }
        Logger.LogInfo(string.Format("SolutionDirectory path is '{0}'", WeavingTask.SolutionDir));
    }
}