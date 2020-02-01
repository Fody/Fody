using System.IO;
using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition = null!;
    public FileStream? SymbolStream;
    string tempAssembly = null!;
    string? tempSymbols;

    public virtual void ReadModule()
    {
        tempAssembly = $"{AssemblyFilePath}.tmp";
        File.Copy(AssemblyFilePath, tempAssembly, true);

        if (debugReaderProvider != null && DebugSymbols != DebugSymbolsType.Embedded)
        {
            tempSymbols = $"{pdbPath}.tmp";
            if (File.Exists(pdbPath))
            {
                File.Copy(pdbPath, tempSymbols, true);
                SymbolStream = FileEx.OpenRead(tempSymbols);
            }
        }

        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver,
            ReadSymbols = SymbolStream != null || DebugSymbols == DebugSymbolsType.Embedded,
            SymbolReaderProvider = debugReaderProvider,
            SymbolStream = SymbolStream,
        };

        ModuleDefinition = ModuleDefinition.ReadModule(tempAssembly, readerParameters);
    }

    void CleanupTempSymbolsAndAssembly()
    {
        SymbolStream?.Dispose();
        File.Delete(tempAssembly);
        if (File.Exists(tempSymbols))
        {
            File.Delete(tempSymbols);
        }
    }
}