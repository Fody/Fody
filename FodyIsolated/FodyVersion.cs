public static class FodyVersion
{
    public static readonly Version Version = typeof(FodyVersion).Assembly.GetName().Version;
    public static readonly int Major = Version.Major;

    public static bool WeaverRequiresUpdate(Assembly assembly, out int referencedVersion)
    {
        var reference = FindFodyHelpersReference(assembly);
        referencedVersion = reference.Version.Major;
        return referencedVersion < Major;
    }

    public static AssemblyName FindFodyHelpersReference(Assembly assembly)
    {
        foreach (var reference in assembly.GetReferencedAssemblies())
        {
            if (reference.Name == "FodyHelpers")
            {
                return reference;
            }
        }
        throw new WeavingException("Weavers must have a reference to FodyHelpers.");
    }
}