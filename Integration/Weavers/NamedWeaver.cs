using System.Collections.Generic;
using Fody;
using Mono.Cecil;

public class NamedWeaver: BaseModuleWeaver
{
    public override void Execute()
    {
        var type = GetType();
        var typeDefinition = new TypeDefinition(
            @namespace: type.Assembly.GetName().Name,
            name: $"TypeInjectedBy{type.Name}",
            attributes: TypeAttributes.Public,
            baseType: TypeSystem.ObjectReference);
        ModuleDefinition.Types.Add(typeDefinition);
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }
}