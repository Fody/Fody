public partial class Processor
{
    public virtual string FindAssemblyPath(string weaverName)
    {
        var assemblyPath = addinFinder.FindAddinAssembly(weaverName);
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