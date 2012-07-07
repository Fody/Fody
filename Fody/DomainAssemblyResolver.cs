using System;
using System.Linq;
using System.Reflection;

public static class DomainAssemblyResolver
{
    public static void Connect()
    {
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
    }

    static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
    }
}