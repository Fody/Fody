public partial class Processor
{
    public virtual string FindAssemblyPath(string weaverName)
    {
        var assemblyPath = FindAddinAssembly(weaverName + ".Fody");
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