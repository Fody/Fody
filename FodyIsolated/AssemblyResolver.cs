using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

public partial class InnerWeaver : IAssemblyResolver
{
    Dictionary<string, AssemblyDefinition> assemblyDefinitionCache = new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCultureIgnoreCase);

    AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
    {
        AssemblyDefinition assemblyDefinition;
        if (assemblyDefinitionCache.TryGetValue(file, out assemblyDefinition))
        {
            return assemblyDefinition;
        }
        if (parameters.AssemblyResolver == null)
        {
            parameters.AssemblyResolver = this;
        }
        try
        {
            assemblyDefinitionCache[file] = assemblyDefinition = ModuleDefinition.ReadModule(file, parameters).Assembly;
            return assemblyDefinition;
        }
        catch (Exception exception)
        {
            throw new Exception(string.Format("Could not read '{0}'.", file), exception);
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
        if (ReferenceDictionary.TryGetValue(assemblyNameReference.Name, out fileFromDerivedReferences))
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

        var joinedReferences = String.Join(Environment.NewLine, SplitReferences.OrderBy(x => x));
        Logger.LogDebug(string.Format("Can not find '{0}'.{1}Tried:{1}{2}", assemblyNameReference.FullName, Environment.NewLine, joinedReferences));
        return null;
    }

    IEnumerable<string> SearchDirForMatchingName(AssemblyNameReference assemblyNameReference)
    {
        var fileName = assemblyNameReference.Name + ".dll";
        return ReferenceDictionary.Values
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
            throw new ArgumentNullException("fullName");
        }

        return Resolve(AssemblyNameReference.Parse(fullName), parameters);
    }

}