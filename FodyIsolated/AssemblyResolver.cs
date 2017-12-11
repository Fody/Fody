using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

public class AssemblyResolver : IAssemblyResolver
{
    Dictionary<string, string> referenceDictionary;
    ILogger logger;
    List<string> splitReferences;
    Dictionary<string, AssemblyDefinition> assemblyDefinitionCache = new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCultureIgnoreCase);

    public AssemblyResolver(Dictionary<string, string> referenceDictionary, ILogger logger, List<string> splitReferences)
    {
        this.referenceDictionary = referenceDictionary;
        this.logger = logger;
        this.splitReferences = splitReferences;
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

    public AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference)
    {
        return Resolve(assemblyNameReference, new ReaderParameters());
    }

    public AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference, ReaderParameters parameters)
    {
        if (parameters == null)
        {
            parameters = new ReaderParameters();
        }

        if (referenceDictionary.TryGetValue(assemblyNameReference.Name, out var fileFromDerivedReferences))
        {
            return GetAssembly(fileFromDerivedReferences, parameters);
        }

        return TryToReadFromDirs(assemblyNameReference, parameters);
    }

    AssemblyDefinition TryToReadFromDirs(AssemblyNameReference assemblyNameReference, ReaderParameters parameters)
    {
        var joinedReferences = string.Join(Environment.NewLine, splitReferences.OrderBy(x => x));
        logger.LogDebug($"Can't find '{assemblyNameReference.FullName}'.{Environment.NewLine}Tried:{Environment.NewLine}{joinedReferences}");
        return null;
    }

    public void Dispose()
    {
        foreach (var value in assemblyDefinitionCache.Values)
        {
            value?.Dispose();
        }
    }
}