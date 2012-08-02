using System.IO;

public class SolutionPathValidator
{
    public ILogger Logger;
    public string SolutionDir;

    public void Execute()
    {
        if (!Directory.Exists(SolutionDir))
        {
            throw new WeavingException(string.Format("SolutionDir \"{0}\" does not exist.", SolutionDir));
        }
        Logger.LogInfo(string.Format("SolutionDirectory path is '{0}'", SolutionDir));
    }
}