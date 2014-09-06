public partial class Processor
{

    public virtual bool ShouldStartSinceFileChanged()
    {
        if (ContainsTypeChecker.Check(AssemblyFilePath, "ProcessedByFody"))
        {
            Logger.LogInfo("Did not process because file has already been processed.");
            return false;
        }
        return true;
    }
}