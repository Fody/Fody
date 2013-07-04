using System;
using System.Reflection;

public static class AppDomainAssemblyFinder
{

    public static void Attach()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName == args.Name)
            {
                return assembly;
            }
        }
        return null;
    }
}