using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

        CecilVersionChecker.VerifyCecilReference(loadFromFile);
        return assemblies[assemblyPath] = loadFromFile;
    }

    public static Assembly LoadFromFile(string assemblyPath)
    {
        var rawAssembly = File.ReadAllBytes(assemblyPath);
        return Assembly.Load(rawAssembly);
    }
}