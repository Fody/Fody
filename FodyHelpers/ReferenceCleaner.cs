static class ReferenceCleaner
{
    public static void CleanReferences(ModuleDefinition module, BaseModuleWeaver weaver, List<string> referenceCopyLocalPaths, List<string> runtimeCopyLocalPaths, Action<string> log)
    {
        if (!weaver.ShouldCleanReference)
        {
            return;
        }

        var weaverLibName = weaver.GetType().Assembly.GetName().Name.ReplaceCaseless(".Fody", "");
        log($"Removing reference to '{weaverLibName}'.");

        var referenceToRemove = module.AssemblyReferences.FirstOrDefault(_ => _.Name == weaverLibName);
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

        referenceCopyLocalPaths.RemoveAll(refPath => copyLocalFilesToRemove.Contains(Path.GetFileName(refPath)));
        runtimeCopyLocalPaths.RemoveAll(refPath => copyLocalFilesToRemove.Contains(Path.GetFileName(refPath)));
    }
}
