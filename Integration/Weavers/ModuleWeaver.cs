using Mono.Cecil;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public void Execute()
    {
        var type = GetType();
        var typeDefinition = new TypeDefinition(
            @namespace: type.Assembly.GetName().Name,
            name: $"TypeInjectedBy{type.Name}",
            attributes: TypeAttributes.Public,
            baseType: ModuleDefinition.ImportReference(typeof(object)));
        ModuleDefinition.Types.Add(typeDefinition);
    }
}