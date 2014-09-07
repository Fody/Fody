using System.IO;

public partial class Processor
{

    public virtual void ValidateSolutionPath()
    {
	    SolutionDirectoryPath = Path.GetFullPath(SolutionDirectoryPath);
        if (!Directory.Exists(SolutionDirectoryPath))
        {
            throw new WeavingException(string.Format("SolutionDir \"{0}\" does not exist.", SolutionDirectoryPath));
        }
        Logger.LogDebug(string.Format("SolutionDirectory path is '{0}'", SolutionDirectoryPath));
    }
}