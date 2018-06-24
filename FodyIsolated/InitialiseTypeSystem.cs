using TypeSystem = Fody.TypeSystem;

public partial class InnerWeaver
{
    public TypeSystem TypeSystem;

    void InitialiseTypeSystem()
    {
        TypeSystem = new TypeSystem(TypeCache.FindType, ModuleDefinition);
        foreach (var weaver in weaverInstances)
        {
            weaver.Instance.TypeSystem = TypeSystem;
        }
    }
}