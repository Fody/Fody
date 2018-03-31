using System;
using System.Collections.Generic;
using Mono.Cecil;

public partial class InnerWeaver
{
    TypeCache typeCache = new TypeCache();

    void BuildAssembliesToScan()
    {
        var assemblyDefinitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var weaver in weaverInstances)
        {
            foreach (var assemblyName in weaver.Instance.GetAssembliesForScanning())
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
        typeCache.Initialise(assemblyDefinitions.Values);
    }
}