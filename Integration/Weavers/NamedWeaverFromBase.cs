using System.Collections.Generic;
using Fody;
using Mono.Cecil;

public class NamedWeaverFromBase : BaseModuleWeaver
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
        var foundType = FindTypeDefinition("System.Boolean");
        if (foundType == null)
        {
            throw new WeavingException("Expected to find System.Boolean");
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
    }
}