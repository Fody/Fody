using System;
using System.Collections.Generic;
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
        return assemblies[assemblyPath] = LoadFromFile(assemblyPath);
    }

    Assembly LoadFromFile(string assemblyPath)
    {
        #if(NETSTANDARD)
        return LoadContext.LoadNotLocked(assemblyPath);
        #else
        var rawAssembly = System.IO.File.ReadAllBytes(assemblyPath);
        return Assembly.Load(rawAssembly);
        #endif
    }
}