using System;
using System.Linq;
using System.Reflection;

public static class DomainAssemblyResolver
{
    public static void Connect() =>
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) => GetAssembly(args.Name);

    public static Assembly? GetAssembly(string name) =>
        AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(_ => string.Equals(_.FullName, name, StringComparison.OrdinalIgnoreCase));
}
