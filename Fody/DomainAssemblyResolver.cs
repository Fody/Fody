using System;
using System.Linq;
using System.Reflection;

public static class DomainAssemblyResolver
{
    public static void Connect()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => GetAssembly(args.Name);
    }

    public static Assembly? GetAssembly(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => string.Equals(x.FullName, name, StringComparison.OrdinalIgnoreCase));
    }
}
