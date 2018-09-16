using System.IO;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;

public partial class InnerWeaver
{
    bool pdbFound;
    bool mdbFound;
    ISymbolReaderProvider debugReaderProvider;
    ISymbolWriterProvider debugWriterProvider;
    string mdbPath;
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
                FindMdb();
                ChooseNewest();

                if (pdbFound)
                {
                    debugReaderProvider = new PdbReaderProvider();
                    debugWriterProvider = new PdbWriterProvider();
                    return;
                }

                if (mdbFound)
                {
                    debugReaderProvider = new MdbReaderProvider();
                    debugWriterProvider = new MdbWriterProvider();
                    return;
                }

                Logger.LogWarning("No debug symbols found. It is recommended to build with debug symbols enabled.");
                return;
            }
        }
    }

    void ChooseNewest()
    {
        if (!pdbFound || !mdbFound)
        {
            return;
        }
        if (File.GetLastWriteTimeUtc(pdbPath) >= File.GetLastWriteTimeUtc(mdbPath))
        {
            mdbFound = false;
            Logger.LogDebug("Found mdb and pdb debug symbols. Selected pdb (newer).");
        }
        else
        {
            pdbFound = false;
            Logger.LogDebug("Found mdb and pdb debug symbols. Selected mdb (newer).");
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

    void FindMdb()
    {
        mdbPath = AssemblyFilePath + ".mdb";
        if (File.Exists(mdbPath))
        {
            mdbFound = true;
            Logger.LogDebug($"Found debug symbols at '{mdbPath}'.");
        }
    }
}