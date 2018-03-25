
using Fody;

public partial class Processor
{
    public virtual string FindAssemblyPath(string weaverName, VersionFilter filter)
    {
        var assemblyPath = addinFinder.FindAddinAssembly(weaverName, filter);
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