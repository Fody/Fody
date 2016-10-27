using System.IO;
using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition;
    public FileStream SymbolStream;

    public virtual void ReadModule()
    {
        string symbolsPath;
        if (pdbFound)
        {
            symbolsPath = pdbPath;
        }
        else
        {
            symbolsPath = mdbPath;
        }

        var tempAssembly = $"{AssemblyFilePath}.tmp";
        var tempSymbols = $"{symbolsPath}.tmp";
        File.Copy(AssemblyFilePath, tempAssembly,true);
        File.Copy(symbolsPath, tempSymbols, true);
        SymbolStream = File.OpenRead(tempSymbols);
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = this,
            ReadSymbols = true,
            SymbolReaderProvider = debugReaderProvider,
            SymbolStream = SymbolStream,
        };
        ModuleDefinition = ModuleDefinition.ReadModule(tempAssembly, readerParameters);
    }
}