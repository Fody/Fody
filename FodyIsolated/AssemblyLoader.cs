using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Fody;

public partial class InnerWeaver
{
    static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

    public Assembly LoadWeaverAssembly(string assemblyPath)
    {
        if (assemblies.TryGetValue(assemblyPath, out var assembly))
        {
            Logger.LogDebug($"  Loading '{assemblyPath}' from cache.");
            return assembly;
        }
        Logger.LogDebug($"  Loading '{assemblyPath}' from disk.");
        var loadFromFile = LoadFromFile(assemblyPath);
        ValidateWeaverReferencesAtLeastTheCurrentMajorOfFody(loadFromFile);
        return assemblies[assemblyPath] = loadFromFile;
    }

    static int FodyMajorVersion = typeof(InnerWeaver).Assembly.GetName().Version.Major;

    public static void ValidateWeaverReferencesAtLeastTheCurrentMajorOfFody(Assembly assembly)
    {
        var reference = FindFodyHelpersReference(assembly);
        if (reference.Version.Major < FodyMajorVersion)
        {
            throw new WeavingException($"Weavers must reference at least the current major version of Fody (version {FodyMajorVersion}). The weaver in {assembly.GetName().Name} references version {reference.Version.Major}.");
        }
    }

    public static AssemblyName FindFodyHelpersReference(Assembly assembly)
    {
        foreach (var reference in assembly.GetReferencedAssemblies())
        {
            if (reference.Name == "FodyHelpers")
            {
                return reference;
            }
        }
        throw new WeavingException("Weavers must have a reference to FodyHelpers.");
    }

    static Assembly LoadFromFile(string assemblyPath)
    {
        var rawAssembly = File.ReadAllBytes(assemblyPath);
        return Assembly.Load(rawAssembly);
    }
}