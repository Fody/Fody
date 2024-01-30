public static class AssemblyResolverHelper
{
    public static AssemblyDefinition? Resolve(this IAssemblyResolver resolver, string assemblyName) =>
        resolver.Resolve(new(assemblyName, null));
}
