using System;
using System.IO;

public class ContainsTypeChecker
{
    static IContainsTypeChecker containsTypeChecker;

    static ContainsTypeChecker()
    {
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = AssemblyLocation.CurrentDirectory(),
        };
        var appDomain = AppDomain.CreateDomain("Fody.ContainsTypeChecker", null, appDomainSetup);
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory(), "FodyIsolated.dll");
        var instanceAndUnwrap = appDomain.CreateInstanceFromAndUnwrap(assemblyFile, "IsolatedContainsTypeChecker");
        containsTypeChecker = (IContainsTypeChecker)instanceAndUnwrap;
    }


    //TODO: possibly cache based on file stamp to avoid cross domain call. need to profile.
    public bool Check(string assemblyPath, string typeName)
    {
        return containsTypeChecker.Check(assemblyPath, typeName);
    }

}