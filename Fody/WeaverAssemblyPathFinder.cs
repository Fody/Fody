public class WeaverAssemblyPathFinder
{
    public ContainsTypeChecker ContainsTypeChecker;
    public AddinFilesEnumerator AddinFilesEnumerator;

    public virtual string FindAssemblyPath(string weaverName)
    {
        var assemblyPath = AddinFilesEnumerator.FindAddinAssembly(weaverName + ".Fody");
        if (assemblyPath != null)
        {
            if (ContainsTypeChecker.Check(assemblyPath, "ModuleWeaver"))
            {
                return assemblyPath;
            }
        }
        return null;
    }
}