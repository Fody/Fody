public partial class Processor
{
    public bool WeaverProjectUsed;

    public virtual bool WeaverProjectContainsType(string weaverName)
    {
        if (FoundWeaverProjectFile)
        {
            var check = ContainsTypeChecker.Check(WeaverAssemblyPath, weaverName);
            if (check)
            {
                WeaverProjectUsed = true;
                return true;
            }
        }
        return false;
    }
}