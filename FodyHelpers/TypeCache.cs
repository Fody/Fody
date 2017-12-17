using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public class TypeCache
{
    Dictionary<string, TypeDefinition> cachedTypes = new Dictionary<string, TypeDefinition>();

    public void Initialise(IEnumerable<AssemblyDefinition> assemblyDefinitions)
    {
        var definitions = assemblyDefinitions.ToList();
        foreach (var assembly in definitions)
        {
            foreach (var type in assembly.MainModule.GetTypes())
            {
                AddIfPublic(type);
            }
        }

        foreach (var assembly in definitions)
        {
            foreach (var exportedType in assembly.MainModule.ExportedTypes)
            {
                if (definitions.All(x => x.Name.Name != exportedType.Scope.Name))
                {
                    AddIfPublic(exportedType.Resolve());
                }
            }
        }
    }

    public virtual TypeDefinition FindType(string typeName)
    {
        if (cachedTypes.TryGetValue(typeName, out var type))
        {
            return type;
        }

        throw new WeavingException($"Could not find '{typeName}'.");
    }

    void AddIfPublic(TypeDefinition type)
    {
        if (!type.IsPublic)
        {
            return;
        }
        if (cachedTypes.ContainsKey(type.FullName))
        {
            return;
        }

        cachedTypes.Add(type.FullName, type);
    }
}