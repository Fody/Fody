using System;
using System.Linq;
using Mono.Cecil;

namespace Fody
{
    [Obsolete(OnlyForTesting.Message)]
    public static class ReferenceCleaner
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
                log($"\tNo reference to '{weaverName}' found. References not modified.");
                return;
            }

            module.AssemblyReferences.Remove(referenceToRemove);
            log($"\tRemoving reference to '{weaverName}'.");
        }
    }
}