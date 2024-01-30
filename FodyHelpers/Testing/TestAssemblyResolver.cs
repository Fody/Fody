namespace Fody;

/// <summary>
/// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
/// </summary>
public class TestAssemblyResolver : IAssemblyResolver
{
    Dictionary<string, AssemblyDefinition> definitions = new(StringComparer.OrdinalIgnoreCase);

    public void Dispose()
    {
        foreach (var definition in definitions.Values)
        {
            definition.Dispose();
        }
    }

    public AssemblyDefinition? Resolve(AssemblyNameReference name) =>
        Resolve(name.Name);

    public AssemblyDefinition? Resolve(string name)
    {
        if (!definitions.TryGetValue(name, out var definition))
        {
            if (!TryGetAssemblyLocation(name, out var assemblyLocation))
            {
                return null;
            }

            definitions[name] = definition = GetAssemblyDefinition(assemblyLocation);
        }

        return definition;
    }

    static bool TryGetAssemblyLocation(
        string name,
        [NotNullWhen(true)] out string? assemblyLocation)
    {
#if (NETSTANDARD2_0)
        if (string.Equals(name, "netstandard", StringComparison.OrdinalIgnoreCase))
        {
            var netstandard = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                @"dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.0\build\netstandard2.0\ref\netstandard.dll");
            if (File.Exists(netstandard))
            {
                assemblyLocation = netstandard;
                return true;
            }
        }
#endif
        var assembly = GetAssembly(name);
        if (assembly == null)
        {
            assemblyLocation = null;
            return false;
        }

        assemblyLocation = assembly.Location;
        return true;
    }

    static Assembly? GetAssembly(string name)
    {
        if (string.Equals(name, "System", StringComparison.OrdinalIgnoreCase))
        {
            return typeof(GeneratedCodeAttribute).Assembly;
        }

        try
        {
            return Assembly.LoadWithPartialName(name);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    AssemblyDefinition GetAssemblyDefinition(string assemblyLocation)
    {
        var readerParameters = new ReaderParameters(ReadingMode.Deferred)
        {
            ReadWrite = false,
            ReadSymbols = false,
            AssemblyResolver = this
        };
        return AssemblyDefinition.ReadAssembly(assemblyLocation, readerParameters);
    }

    public AssemblyDefinition? Resolve(AssemblyNameReference name, ReaderParameters parameters) =>
        Resolve(name);
}