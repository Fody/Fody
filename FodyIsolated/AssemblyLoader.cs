using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public partial class InnerWeaver
{
    static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

    public virtual Assembly LoadAssembly(string assemblyPath)
    {
        if (assemblies.TryGetValue(assemblyPath, out var assembly))
        {
            Logger.LogDebug($"  Loading '{assemblyPath}' from cache.");
            return assembly;
        }
        Logger.LogDebug($"  Loading '{assemblyPath}' from disk.");
        var loadFromFile = LoadFromFile(assemblyPath);

        VerifyCecilReference(loadFromFile);
        return assemblies[assemblyPath] = loadFromFile;
    }

    public static Assembly LoadFromFile(string assemblyPath)
    {
        var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
        var rawAssembly = File.ReadAllBytes(assemblyPath);
        if (File.Exists(pdbPath))
        {
            return Assembly.Load(rawAssembly, File.ReadAllBytes(pdbPath));
        }
        var mdbPath = Path.ChangeExtension(assemblyPath, "mdb");
        if (File.Exists(mdbPath))
        {
            return Assembly.Load(rawAssembly, File.ReadAllBytes(mdbPath));
        }
        return Assembly.Load(rawAssembly);
    }
}