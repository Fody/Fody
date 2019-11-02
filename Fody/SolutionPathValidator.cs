using System.IO;

public partial class Processor
{
    public virtual void ValidateSolutionPath()
    {
        SolutionDirectory = Path.GetFullPath(SolutionDirectory);
        if (!Directory.Exists(SolutionDirectory))
        {
            throw new WeavingException($"SolutionDir '{SolutionDirectory}' does not exist.");
        }
        Logger.LogDebug($"SolutionDirectory path is '{SolutionDirectory}'");
    }
}