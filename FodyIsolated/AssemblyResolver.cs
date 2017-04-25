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

    public AssemblyResolver(Dictionary<string, string> ReferenceDictionary, ILogger logger, List<string> splitReferences)
    {
        referenceDictionary = ReferenceDictionary;
        this.logger = logger;
        this.splitReferences = splitReferences;
    }

    AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
    {
        AssemblyDefinition assembly;
        if (assemblyDefinitionCache.TryGetValue(file, out assembly))
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

        string fileFromDerivedReferences;
        if (referenceDictionary.TryGetValue(assemblyNameReference.Name, out fileFromDerivedReferences))
        {
            return GetAssembly(fileFromDerivedReferences, parameters);
        }

        return TryToReadFromDirs(assemblyNameReference, parameters);
    }

    AssemblyDefinition TryToReadFromDirs(AssemblyNameReference assemblyNameReference, ReaderParameters parameters)
    {
        var filesWithMatchingName = SearchDirForMatchingName(assemblyNameReference).ToList();
        foreach (var filePath in filesWithMatchingName)
        {
            var assemblyName = AssemblyName.GetAssemblyName(filePath);
            if (assemblyNameReference.Version == null || assemblyName.Version == assemblyNameReference.Version)
            {
                return GetAssembly(filePath, parameters);
            }
        }
        foreach (var filePath in filesWithMatchingName.OrderByDescending(s => AssemblyName.GetAssemblyName(s).Version))
        {
            return GetAssembly(filePath, parameters);
        }

        var joinedReferences = string.Join(Environment.NewLine, splitReferences.OrderBy(x => x));
        logger.LogDebug(string.Format("Can not find '{0}'.{1}Tried:{1}{2}", assemblyNameReference.FullName, Environment.NewLine, joinedReferences));
        return null;
    }

    IEnumerable<string> SearchDirForMatchingName(AssemblyNameReference assemblyNameReference)
    {
        var fileName = assemblyNameReference.Name + ".dll";
        return referenceDictionary.Values
            .Select(x => Path.Combine(Path.GetDirectoryName(x), fileName))
            .Where(File.Exists);
    }

    public AssemblyDefinition Resolve(string fullName)
    {
        return Resolve(AssemblyNameReference.Parse(fullName));
    }

    public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
    {
        if (fullName == null)
        {
            throw new ArgumentNullException(nameof(fullName));
        }

        return Resolve(AssemblyNameReference.Parse(fullName), parameters);
    }

    public void Dispose()
    {
        foreach (var value in assemblyDefinitionCache.Values)
        {
            value?.Dispose();
        }
    }
}