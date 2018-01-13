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

        if (FindFromValues(typeName, out type))
        {
            return type;
        }

        throw new WeavingException($"Could not find '{typeName}'.");
    }

   bool FindFromValues(string typeName, out TypeDefinition type)
    {
        if (!typeName.Contains('.'))
        {
            var types = cachedTypes.Values
                .Where(x=>x.Name == typeName)
                .ToList();
            if (types.Count > 1)
            {
                throw new WeavingException($"Found multiple types for '{typeName}'.");
            }
            if (types.Count ==0)
            {
                type = null;
                return false;
            }

            type = types[0];
            return true;
        }

        type = null;
        return false;
    }

    public virtual bool TryFindType(string typeName, out TypeDefinition type)
    {
        if (cachedTypes.TryGetValue(typeName, out type))
        {
            return true;
        }

        return FindFromValues(typeName, out type);
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