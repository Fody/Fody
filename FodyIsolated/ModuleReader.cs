using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition = null!;
    bool hasSymbols;

    public virtual void ReadModule()
    {
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver,
            InMemory = true
        };

        ModuleDefinition = ModuleDefinition.ReadModule(AssemblyFilePath, readerParameters);

        try
        {
            ModuleDefinition.ReadSymbols();
            hasSymbols = true;
        }
        catch
        {
            Logger.LogInfo("Debug symbols disabled.");
        }
    }
}