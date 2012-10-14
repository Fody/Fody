using System.IO;

public partial class Processor
{

    public void ValidateSolutionPath()
    {
        if (!Directory.Exists(SolutionDirectoryPath))
        {
            throw new WeavingException(string.Format("SolutionDir \"{0}\" does not exist.", SolutionDirectoryPath));
        }
        Logger.LogInfo(string.Format("SolutionDirectory path is '{0}'", SolutionDirectoryPath));
    }
}