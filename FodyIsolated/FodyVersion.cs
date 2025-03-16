public static class FodyVersion
{
    public static readonly Version Version = typeof(FodyVersion).Assembly.GetName().Version ?? new Version();
    public static readonly int Major = Version.Major;

    public static bool WeaverRequiresUpdate(Assembly assembly, out int referencedVersion)
    {
        var version = FindFodyHelpersReference(assembly).Version;
        if (version == null)
        {
            referencedVersion = 0;
            return true;
        }
        referencedVersion = version.Major;
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