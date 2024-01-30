using System.Reflection;

public partial class InnerWeaver
{
    static Dictionary<string, Assembly> assemblies = new(StringComparer.OrdinalIgnoreCase);

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

    // ReSharper disable once MemberCanBeMadeStatic.Local
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