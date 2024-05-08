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

    Assembly LoadFromFile(string assemblyPath)
    {
        try
        {
#if NETSTANDARD
            return LoadContext.LoadNotLocked(assemblyPath);
#else
            var rawAssembly = File.ReadAllBytes(assemblyPath);
            return Assembly.Load(rawAssembly);
#endif
        }
        catch (Exception ex)
        {
            throw new WeavingException($"Could not load weaver assembly from {assemblyPath}: {ex.Message}");
        }
    }
}
