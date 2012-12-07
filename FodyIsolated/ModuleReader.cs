using System.IO;
using Mono.Cecil;

public partial class InnerWeaver
{
    public ModuleDefinition ModuleDefinition;

    FileStream GetSymbolReaderProvider(string assemblyPath)
    {
        var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
        if (File.Exists(pdbPath))
        {
            Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", pdbPath));
            return File.OpenRead(pdbPath);
        }
        var mdbPath = assemblyPath + ".mdb";

        if (File.Exists(mdbPath))
        {
            Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", mdbPath));
            return File.OpenRead(mdbPath);
        }

        Logger.LogInfo("Found no debug symbols.");
        return null;
    }

    public void ReadModule()
    {
        using (var symbolStream = GetSymbolReaderProvider(AssemblyFilePath))
        {
            var readSymbols = symbolStream != null;
            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = this,
                ReadSymbols = readSymbols,
                SymbolStream = symbolStream,
            };
            ModuleDefinition = ModuleDefinition.ReadModule(AssemblyFilePath, readerParameters);
        }
    }
}