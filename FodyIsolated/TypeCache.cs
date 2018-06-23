using System;
using System.Collections.Generic;
using Mono.Cecil;

public partial class InnerWeaver
{
    TypeCache typeCache = new TypeCache();

    static List<string> defaultAssemblies = new List<string>()
    {
        "mscorlib",
        "System",
        "System.Runtime",
        "System.Core",
        "netstandard"
    };

    void BuildAssembliesToScan()
    {
        var assemblyDefinitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var assemblyName in GetAssembliesForScanning())
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

        typeCache.Initialise(assemblyDefinitions.Values);
    }

    IEnumerable<string> GetAssembliesForScanning()
    {
        foreach (var assemblyName in defaultAssemblies)
        {
            yield return assemblyName;
        }

        foreach (var weaver in weaverInstances)
        {
            foreach (var assemblyName in weaver.Instance.GetAssembliesForScanning())
            {
                yield return assemblyName;
            }
        }
    }
}