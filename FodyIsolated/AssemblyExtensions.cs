static class AssemblyExtensions
{
    public static string GetVersion(this Assembly assembly)
    {
        var informationalVersion = GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(assembly);
        if (informationalVersion != null)
        {
            return informationalVersion.InformationalVersion;
        }

        var version = assembly.GetName().Version;
        if (version != null)
        {
            return version.ToString();
        }

        return "unknown";
    }

    private static TAttibute? GetAssemblyAttribute<TAttibute>(Assembly assembly)
        where TAttibute : Attribute
    {
        var attibutes = assembly.GetCustomAttributes(typeof(TAttibute))?.ToArray() ?? [];
        return attibutes.Length > 0 ? attibutes[0] as TAttibute : null;
    }
}
