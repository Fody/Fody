using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
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
                var assembly = GetAssembly(name);
                if (assembly == null)
                {
                    return null;
                }
                definitions[name] = definition = GetAssemblyDefinition(assembly);
            }

            return definition;
        }

        static Assembly GetAssembly(string name)
        {
#if (NETSTANDARD2_0)
            if (string.Equals(name, "netstandard", StringComparison.OrdinalIgnoreCase))
            {
            var netstandard = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                @"dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.0\build\netstandard2.0\ref\netstandard.dll");
                if (File.Exists(netstandard))
                {
                    return Assembly.LoadFrom(netstandard);
                }
                throw new Exception($"Expected netstandard 2.0 to exist for testing purposes: {netstandard}");
            }
#endif
            if (string.Equals(name, "System", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(GeneratedCodeAttribute).Assembly;
            }

            try
            {
#pragma warning disable 618
                return Assembly.LoadWithPartialName(name);
#pragma warning restore 618
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        AssemblyDefinition GetAssemblyDefinition(Assembly assembly)
        {
            var assemblyLocation = assembly.GetAssemblyLocation();
            var readerParameters = new ReaderParameters(ReadingMode.Deferred)
            {
                ReadWrite = false,
                ReadSymbols = false,
                AssemblyResolver = this
            };
            return AssemblyDefinition.ReadAssembly(assemblyLocation, readerParameters);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name);
        }
    }
}