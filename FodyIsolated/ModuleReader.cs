using Fody;
using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition;

    public virtual void ReadModule()
    {
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver,
            ReadSymbols = true,
#pragma warning disable 618
            SymbolReaderProvider = new SymbolReaderProvider(),
#pragma warning restore 618
            ReadWrite = true
        };
        ModuleDefinition = ModuleDefinition.ReadModule(AssemblyFilePath, readerParameters);
    }
}