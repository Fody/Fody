using System;
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

        var weaverName = weaver.GetType().Assembly.GetName().Name.ReplaceCaseless(".Fody", "");
        var referenceToRemove = module.AssemblyReferences
            .FirstOrDefault(x => x.Name == weaverName);
        if (referenceToRemove == null)
        {
            return;
        }

        module.AssemblyReferences.Remove(referenceToRemove);
        log($"\tRemoving reference to '{weaverName}'.");
    }
}