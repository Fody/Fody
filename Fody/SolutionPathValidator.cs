using System.IO;

public partial class Processor
{

    public virtual void ValidateSolutionPath()
    {
	    SolutionDirectory = Path.GetFullPath(SolutionDirectory);
        if (!Directory.Exists(SolutionDirectory))
        {
            throw new WeavingException(string.Format("SolutionDir \"{0}\" does not exist.", SolutionDirectory));
        }
        Logger.LogDebug(string.Format("SolutionDirectory path is '{0}'", SolutionDirectory));
    }
}