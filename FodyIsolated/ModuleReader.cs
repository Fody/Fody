using System.IO;
using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition;
    public FileStream SymbolStream;

    public virtual void ReadModule()
    {
        var tempAssembly = $"{AssemblyFilePath}.tmp";
        File.Copy(AssemblyFilePath, tempAssembly, true);

        if (debugReaderProvider != null)
        {
            var symbolsPath = pdbFound ? pdbPath : mdbPath;
            var tempSymbols = $"{symbolsPath}.tmp";
            if (File.Exists(symbolsPath))
            {
                File.Copy(symbolsPath, tempSymbols, true);
                SymbolStream = File.OpenRead(tempSymbols);
            }
        }

        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver,
            ReadSymbols = SymbolStream != null,
            SymbolReaderProvider = debugReaderProvider,
            SymbolStream = SymbolStream,
        };
        ModuleDefinition = ModuleDefinition.ReadModule(tempAssembly, readerParameters);
    }
}