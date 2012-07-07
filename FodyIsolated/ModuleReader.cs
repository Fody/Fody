using System.IO;
using Mono.Cecil;

public class ModuleReader
{
    public IAssemblyResolver AssemblyResolver;
    public ModuleDefinition ModuleDefinition;
    public ILogger Logger;
    public InnerWeaver InnerWeaver;


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

    public void Execute()
    {
        using (var symbolStream = GetSymbolReaderProvider(InnerWeaver.AssemblyPath))
        {
            var readSymbols = symbolStream != null;
            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = AssemblyResolver,
                ReadSymbols = readSymbols,
                SymbolStream = symbolStream,
            };
            ModuleDefinition = ModuleDefinition.ReadModule(InnerWeaver.AssemblyPath, readerParameters);
        }
    }
}