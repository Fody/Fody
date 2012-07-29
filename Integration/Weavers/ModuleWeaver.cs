using Mono.Cecil;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public void Execute()
    {
        var typeDefinition = new TypeDefinition(GetType().Assembly.GetName().Name, "TypeInjectedBy" + GetType().Name, TypeAttributes.Public, ModuleDefinition.Import(typeof (object)));
        ModuleDefinition.Types.Add(typeDefinition);
    }
}