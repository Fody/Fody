using System.IO;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

public partial class InnerWeaver
{
    bool pdbFound;
    ISymbolReaderProvider debugReaderProvider;
    ISymbolWriterProvider debugWriterProvider;
    string pdbPath;

    void GetSymbolProviders()
    {
        switch (DebugSymbols)
        {
            case DebugSymbolsType.None:
            {
                Logger.LogInfo("Debug symbols disabled.");
                return;
            }

            case DebugSymbolsType.Embedded:
            {
                debugReaderProvider = new EmbeddedPortablePdbReaderProvider();
                debugWriterProvider = new EmbeddedPortablePdbWriterProvider();
                return;
            }

            default:
            {
                FindPdb();

                if (pdbFound)
                {
                    debugReaderProvider = new PdbReaderProvider();
                    debugWriterProvider = new PdbWriterProvider();
                    return;
                }

                Logger.LogWarning("No debug symbols found. It is recommended to build with debug symbols enabled.");
                return;
            }
        }
    }

    void FindPdb()
    {
        // because UWP use a wacky convention for symbols
        pdbPath = Path.ChangeExtension(AssemblyFilePath, "compile.pdb");
        if (File.Exists(pdbPath))
        {
            pdbFound = true;
            Logger.LogDebug($"Found debug symbols at '{pdbPath}'.");
            return;
        }

        pdbPath = Path.ChangeExtension(AssemblyFilePath, "pdb");
        if (File.Exists(pdbPath))
        {
            pdbFound = true;
            Logger.LogDebug($"Found debug symbols at '{pdbPath}'.");
        }
    }
}