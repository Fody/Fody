public class WeaverAssemblyPathFinder
{
    public ContainsTypeChecker ContainsTypeChecker;
    public AddinFilesEnumerator AddinFilesEnumerator;

    public virtual string FindAssemblyPath(string weaverName)
    {
        var assemblyPath = AddinFilesEnumerator.FindAddinAssembly(weaverName + ".Fody");
        if (assemblyPath != null)
        {
            var check = ContainsTypeChecker.Check(assemblyPath, "ModuleWeaver");
            if (check)
            {
                return assemblyPath;
            }
        }
        return null;
    }
}