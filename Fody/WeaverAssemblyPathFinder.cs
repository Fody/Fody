
public partial class Processor
{
    public virtual string FindAssemblyPath(string weaverName)
    {
        var assemblyPath = addinFinder.FindAddinAssembly(weaverName);
        if (assemblyPath != null)
        {
            if (ContainsTypeChecker.Check(assemblyPath, "ModuleWeaver"))
            {
                Logger.LogInfo($"Searched for '{weaverName}'. Found: {assemblyPath}");
                return assemblyPath;
            }
        }
        return null;
    }
}