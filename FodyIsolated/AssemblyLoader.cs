using System.IO;
using System.Reflection;

public static class AssemblyLoader
{

    public static Assembly Load(string assemblyPath)
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