using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

class TypeCache
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
                if (definitions.Any(x => x.Name.Name == exportedType.Scope.Name))
                {
                    continue;
                }

                var typeDefinition = exportedType.Resolve();
                if (typeDefinition == null)
                {
                    continue;
                }

                AddIfPublic(typeDefinition);
            }
        }
    }

    public virtual TypeDefinition FindType(string typeName)
    {
        if (cachedTypes.TryGetValue(typeName, out var type))
        {
            return type;
        }

        if (!typeName.Contains('.'))
        {
            foreach (var typeDefinition in cachedTypes.Values)
            {
                if (typeDefinition.Name == typeName)
                {
                    return typeDefinition;
                }
            }
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