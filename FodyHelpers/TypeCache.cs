using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mono.Cecil;

namespace Fody
{
    /// <summary>
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public class TypeCache
    {
        Func<string, AssemblyDefinition?> resolve;

        public static List<string> defaultAssemblies = new List<string>
        {
            "mscorlib",
            "System",
            "System.Runtime",
            "System.Core",
            "netstandard"
        };

        Dictionary<string, TypeDefinition> cachedTypes = new Dictionary<string, TypeDefinition>();

        public TypeCache(Func<string, AssemblyDefinition?> resolve)
        {
            this.resolve = resolve;
        }

        public void BuildAssembliesToScan(BaseModuleWeaver weaver)
        {
            BuildAssembliesToScan(new[] {weaver});
        }

        public void BuildAssembliesToScan(IEnumerable<BaseModuleWeaver> weavers)
        {
            var assemblyDefinitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (var assemblyName in GetAssembliesForScanning(weavers))
            {
                var assembly = resolve(assemblyName);
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

            Initialise(assemblyDefinitions.Values);
        }

        IEnumerable<string> GetAssembliesForScanning(IEnumerable<BaseModuleWeaver> weavers)
        {
            foreach (var assemblyName in defaultAssemblies)
            {
                yield return assemblyName;
            }

            foreach (var weaver in weavers)
            {
                foreach (var assemblyName in weaver.GetAssembliesForScanning())
                {
                    yield return assemblyName;
                }
            }
        }

        void Initialise(IEnumerable<AssemblyDefinition> assemblyDefinitions)
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
            if (cachedTypes.TryGetValue(typeName, out var cacheType))
            {
                return cacheType;
            }

            if (FindFromValues(typeName, out var fromValueType))
            {
                return fromValueType;
            }

            throw new WeavingException($"Could not find '{typeName}'.");
        }

        bool FindFromValues(string typeName, [NotNullWhen(true)] out TypeDefinition? type)
        {
            if (!typeName.Contains('.'))
            {
                var types = cachedTypes.Values
                    .Where(x => x.Name == typeName)
                    .ToList();
                if (types.Count > 1)
                {
                    throw new WeavingException($"Found multiple types for '{typeName}'.");
                }

                if (types.Count == 0)
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

        public virtual bool TryFindType(string typeName, [NotNullWhen(true)] out TypeDefinition? type)
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
}