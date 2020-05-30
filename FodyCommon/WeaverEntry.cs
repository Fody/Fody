using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class WeaverEntry
{
    const string FodyFileNameSuffix = ".Fody";

    public static readonly IEqualityComparer<WeaverEntry> NameComparer = new WeaverNameComparer();

    /// <summary>
    /// The execution order of this weaver.
    /// </summary>
    public int ExecutionOrder;

    /// <summary>
    /// The content of the XML element containing the configuration.
    /// </summary>
    public string? Element;

    /// <summary>
    /// The source of the configuration element.
    /// </summary>
    public string? ConfigurationSource;

    /// <summary>
    /// The name of the element containing the configuration.
    /// </summary>
    public string ElementName => ConfiguredTypeName ?? AssemblyBaseName;

    /// <summary>
    /// The assembly name including the ".Fody" suffix.
    /// </summary>
    public string WeaverName => Path.GetFileNameWithoutExtension(AssemblyPath);

    /// <summary>
    /// The assembly name excluding the ".Fody" suffix.
    /// </summary>
    string AssemblyBaseName => ExtractAssemblyBaseName(AssemblyPath);

    /// <summary>
    /// The full path to the weaver assembly.
    /// </summary>
    public string AssemblyPath = null!;

    /// <summary>
    /// The type name of the weaver class.
    /// </summary>
    public string TypeName => ConfiguredTypeName ?? "ModuleWeaver";

    /// <summary>
    /// The type name of the weaver class as read from the configuration; maybe <c>null</c> to use the default "ModuleWeaver".
    /// </summary>
    public string? ConfiguredTypeName;

    /// <summary>
    /// True if a PackageReference element item matching the weaver has been found.
    /// </summary>
    public bool HasPackageReference => PrivateAssets != null;

    /// <summary>
    /// The value of the PrivateAssets metadata item for the package reference.
    /// It will be an empty string when the metadata item is missing, and null if the package reference is not found.
    /// </summary>
    public string? PrivateAssets;

    /// <summary>
    /// The value of the IncludeAssets metadata item for the package reference.
    /// It will be an empty string when the metadata item is missing, and null if the package reference is not found.
    /// </summary>
    public string? IncludeAssets;

    static string ExtractAssemblyBaseName(string assemblyPath)
    {
        var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);

        var extension = Path.GetExtension(assemblyName);

        if (string.Equals(extension, FodyFileNameSuffix, StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileNameWithoutExtension(assemblyName);
        }

        return assemblyName;
    }

    class WeaverNameComparer : IEqualityComparer<WeaverEntry>
    {
        public bool Equals(WeaverEntry? x, WeaverEntry? y)
        {
            return x?.ElementName == y?.ElementName;
        }

        public int GetHashCode(WeaverEntry obj)
        {
            return obj.ElementName?.GetHashCode() ?? 0;
        }
    }
}
