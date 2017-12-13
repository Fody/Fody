using System;
using System.Collections.Generic;
using Fody;
using Mono.Cecil;

public partial class InnerWeaver
{
    public Dictionary<string, TypeDefinition> CachedTypes = new Dictionary<string, TypeDefinition>();

    public virtual TypeDefinition FindType(string typeName)
    {
        if (CachedTypes.TryGetValue(typeName, out var type))
        {
            return type;
        }

        throw new WeavingException($"Could not find '{typeName}'.");
    }

    void BuildAssembliesToScan()
    {
        var assemblyDefinitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var weaver in weaverInstances)
        {
            foreach (var assemblyName in weaver.WeaverDelegate.GetAssembliesForScanning(weaver.Instance))
            {
                var assembly = assemblyResolver.Resolve(assemblyName);
                if (assembly == null)
                {
                    continue;
                }

                if (assemblyDefinitions.ContainsKey(assemblyName))
                {
                    continue;
                }
                assemblyDefinitions.Add(assemblyName, assembly);
            }
        }

        foreach (var assembly in assemblyDefinitions.Values)
        {
            foreach (var type in assembly.MainModule.GetTypes())
            {
                AddIfPublic(type);
            }
        }

        foreach (var assembly in assemblyDefinitions.Values)
        {
            foreach (var exportedType in assembly.MainModule.ExportedTypes)
            {
                if (assemblyDefinitions.ContainsKey(exportedType.Scope.Name))
                {
                    AddIfPublic(exportedType.Resolve());
                }
            }
        }
    }

    void AddIfPublic(TypeDefinition type)
    {
        if (!type.IsPublic)
        {
            return;
        }
        if (CachedTypes.ContainsKey(type.FullName))
        {
            return;
        }

        CachedTypes.Add(type.FullName, type);
    }
}