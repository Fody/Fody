using Mono.Cecil;

public static class AssemblyResolverHelper
{
    public static AssemblyDefinition? Resolve(this IAssemblyResolver resolver, string assemblyName)
    {
        return resolver.Resolve(new AssemblyNameReference(assemblyName, null));
    }
}
