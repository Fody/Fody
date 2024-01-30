public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition = null!;
    bool hasSymbols;

    public void ReadModule()
    {
        var result = ReadModule(AssemblyFilePath, assemblyResolver);
        hasSymbols = result.hasSymbols;
        if (!hasSymbols)
        {
            Logger.LogInfo("Module has no debug symbols.");
        }

        ModuleDefinition = result.module;
    }

    public static (ModuleDefinition module, bool hasSymbols) ReadModule(
        string assemblyFilePath,
        IAssemblyResolver assemblyResolver)
    {
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver,
            InMemory = true
        };

        var module = ModuleDefinition.ReadModule(assemblyFilePath, readerParameters);

        var hasSymbols = false;
        try
        {
            module.ReadSymbols();
            hasSymbols = true;
        }
        catch
        {
        }

        return (module, hasSymbols);
    }
}