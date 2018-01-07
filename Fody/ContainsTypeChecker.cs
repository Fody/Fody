using System;
using System.IO;

public class ContainsTypeChecker
{
    static IContainsTypeChecker containsTypeChecker;

    static ContainsTypeChecker()
    {
        var loadContext = new IsolatedAssemblyLoadContext("Fody.ContainsTypeChecker", AssemblyLocation.CurrentDirectory);
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        if (!File.Exists(assemblyFile))
        {
            throw new Exception("Could not find: " + assemblyFile);
        }
        var instanceAndUnwrap = loadContext.CreateInstanceFromAndUnwrap(assemblyFile, "IsolatedContainsTypeChecker");
        containsTypeChecker = (IContainsTypeChecker) instanceAndUnwrap;
    }


    //TODO: possibly cache based on file stamp to avoid cross domain call. need to profile.
    public virtual bool Check(string assemblyPath, string typeName)
    {
        return containsTypeChecker.Check(assemblyPath, typeName);
    }
}