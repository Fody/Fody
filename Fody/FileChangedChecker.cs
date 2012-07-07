using Fody;

public class FileChangedChecker
{
    public ContainsTypeChecker ContainsTypeChecker;
    public ILogger Logger;
    public WeavingTask WeavingTask;

    public bool ShouldStart()
    {
        if (ContainsTypeChecker.Check(WeavingTask.AssemblyPath, "ProcessedByFody"))
        {
            Logger.LogInfo("Did not process because file has already been processed.");
            return false;
        }
        return true;
    }
}