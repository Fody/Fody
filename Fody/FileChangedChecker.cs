public class FileChangedChecker
{
    public ContainsTypeChecker ContainsTypeChecker;
    public ILogger Logger;
    public string AssemblyPath;

    public bool ShouldStart()
    {
        if (ContainsTypeChecker.Check(AssemblyPath, "ProcessedByFody"))
        {
            Logger.LogInfo("Did not process because file has already been processed.");
            return false;
        }
        return true;
    }
}