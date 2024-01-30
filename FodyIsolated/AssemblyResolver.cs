public class AssemblyResolver : IAssemblyResolver
{
    Dictionary<string, string> referenceDictionary;
    ILogger logger;
    Dictionary<string, AssemblyDefinition> assemblyDefinitionCache = new(StringComparer.InvariantCultureIgnoreCase);

    public AssemblyResolver(ILogger logger, IEnumerable<string> splitReferences)
    {
        referenceDictionary = new();
        this.logger = logger;

        foreach (var filePath in splitReferences)
        {
            referenceDictionary[GetAssemblyName(filePath)] = filePath;
        }
    }

    string GetAssemblyName(string filePath)
    {
        try
        {
            return GetAssembly(filePath, new(ReadingMode.Deferred)).Name.Name;
        }
        catch (Exception ex)
        {
            logger.LogDebug($"Could not load {filePath}, assuming the assembly name is equal to the file name: {ex}");
            return Path.GetFileNameWithoutExtension(filePath);
        }
    }

    AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
    {
        if (assemblyDefinitionCache.TryGetValue(file, out var assembly))
        {
            return assembly;
        }

        parameters.AssemblyResolver ??= this;
        try
        {
            return assemblyDefinitionCache[file] = AssemblyDefinition.ReadAssembly(file, parameters);
        }
        catch (Exception exception)
        {
            throw new($"Could not read '{file}'.", exception);
        }
    }

    public virtual AssemblyDefinition? Resolve(AssemblyNameReference assemblyNameReference) =>
        Resolve(assemblyNameReference, new());

    public virtual AssemblyDefinition? Resolve(AssemblyNameReference assemblyNameReference, ReaderParameters? parameters)
    {
        parameters ??= new();

        if (referenceDictionary.TryGetValue(assemblyNameReference.Name, out var fileFromDerivedReferences))
        {
            return GetAssembly(fileFromDerivedReferences, parameters);
        }

        return null;
    }

    public virtual void Dispose()
    {
        foreach (var value in assemblyDefinitionCache.Values)
        {
            value?.Dispose();
        }
    }
}