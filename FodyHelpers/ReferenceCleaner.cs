using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fody;
using Mono.Cecil;

static class ReferenceCleaner
{
    public static void CleanReferences(ModuleDefinition module, BaseModuleWeaver weaver, Action<string> log)
    {
        if (!weaver.ShouldCleanReference)
        {
            return;
        }

        var weaverLibName = weaver.GetType().Assembly.GetName().Name.ReplaceCaseless(".Fody", "");
        log($"Removing reference to '{weaverLibName}'.");

        var referenceToRemove = module.AssemblyReferences.FirstOrDefault(x => x.Name == weaverLibName);
        if (referenceToRemove != null)
        {
            module.AssemblyReferences.Remove(referenceToRemove);
        }

        var copyLocalFilesToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            weaverLibName + ".dll",
            weaverLibName + ".xml",
            weaverLibName + ".pdb"
        };

        weaver.ReferenceCopyLocalPaths.RemoveAll(refPath => copyLocalFilesToRemove.Contains(Path.GetFileName(refPath)));
    }
}
