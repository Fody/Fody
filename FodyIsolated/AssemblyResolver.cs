using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

public class AssemblyResolver : IAssemblyResolver
{
    Dictionary<string, string> referenceDictionary;
    ILogger logger;
    List<string> splitReferences;
    Dictionary<string, AssemblyDefinition> assemblyDefinitionCache = new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCultureIgnoreCase);

    public AssemblyResolver()
    {
    }

    public AssemblyResolver(ILogger logger, IEnumerable<string> splitReferences)
    {
        referenceDictionary = new Dictionary<string, string>();
        this.logger = logger;
        this.splitReferences = splitReferences.ToList();

        foreach (var filePath in this.splitReferences)
        {
            referenceDictionary[GetAssemblyName(filePath)] = filePath;
        }
    }

    string GetAssemblyName(string filePath)
    {
        try
        {
            return GetAssembly(filePath, new ReaderParameters(ReadingMode.Deferred)).Name.Name;
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
        if (parameters.AssemblyResolver == null)
        {
            parameters.AssemblyResolver = this;
        }
        try
        {
            return assemblyDefinitionCache[file] = AssemblyDefinition.ReadAssembly(file, parameters);
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not read '{file}'.", exception);
        }
    }

    public virtual AssemblyDefinition Resolve(string assemblyName)
    {
        return Resolve(new AssemblyNameReference(assemblyName, null));
    }

    public virtual AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference)
    {
        return Resolve(assemblyNameReference, new ReaderParameters());
    }

    public virtual AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference, ReaderParameters parameters)
    {
        if (parameters == null)
        {
            parameters = new ReaderParameters();
        }

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