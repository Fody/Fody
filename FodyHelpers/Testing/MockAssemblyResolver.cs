using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace Fody
{
    class MockAssemblyResolver : IAssemblyResolver
    {
        Dictionary<string, AssemblyDefinition> definitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);

        public void Dispose()
        {
            foreach (var definition in definitions.Values)
            {
                definition.Dispose();
            }
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name.Name);
        }

        public AssemblyDefinition Resolve(string name)
        {
            if (!definitions.TryGetValue(name, out var definition))
            {
                var assembly = Assembly.Load(name);
                if (assembly == null)
                {
                    return null;
                }
                var assemblyLocation = assembly.GetAssemblyLocation();
                var readerParameters = new ReaderParameters(ReadingMode.Deferred)
                {
                    ReadWrite = false,
                    ReadSymbols = false,
                    AssemblyResolver = this
                };
                definitions[name] = definition = AssemblyDefinition.ReadAssembly(assemblyLocation, readerParameters);
            }

            return definition;
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name);
        }
    }
}