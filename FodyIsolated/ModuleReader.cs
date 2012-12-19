using System.IO;
using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition;

    public void ReadModule()
    {
        if (pdbFound)
        {
            using (var symbolStream = File.OpenRead(pdbPath))
            {
                var readerParameters = new ReaderParameters
                    {
                        AssemblyResolver = this,
                        ReadSymbols = pdbFound || mdbFound,
                        SymbolReaderProvider = debugReaderProvider,
                        SymbolStream = symbolStream
                    };
                ModuleDefinition = ModuleDefinition.ReadModule(AssemblyFilePath, readerParameters);
            }
        }
        else
        {
            var readerParameters = new ReaderParameters
                {
                    AssemblyResolver = this,
                    ReadSymbols = pdbFound || mdbFound,
                    SymbolReaderProvider = debugReaderProvider,

                };
            ModuleDefinition = ModuleDefinition.ReadModule(AssemblyFilePath, readerParameters);
        }
    }
}