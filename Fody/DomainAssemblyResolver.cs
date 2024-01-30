public static class DomainAssemblyResolver
{
    public static void Connect() =>
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) => GetAssembly(args.Name);

    public static Assembly? GetAssembly(string name) =>
        AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => string.Equals(x.FullName, name, StringComparison.OrdinalIgnoreCase));
}
