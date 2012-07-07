public class WeaverProjectContainsWeaverChecker
{
    public ContainsTypeChecker ContainsTypeChecker;
    public WeaverProjectFileFinder WeaverProjectFileFinder;
    public bool WeaverProjectUsed;


    public virtual bool WeaverProjectContainsType(string weaverName)
    {
        if (WeaverProjectFileFinder.Found)
        {
            var check = ContainsTypeChecker.Check(WeaverProjectFileFinder.WeaverAssemblyPath, weaverName);
            if (check)
            {
                WeaverProjectUsed = true;
                return true;
            }
        }
        return false;
    }
}