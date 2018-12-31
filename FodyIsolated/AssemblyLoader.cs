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

    static Assembly LoadFromFile(string assemblyPath)
    {
        var rawAssembly = File.ReadAllBytes(assemblyPath);

        var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
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