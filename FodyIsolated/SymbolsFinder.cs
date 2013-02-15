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
		pdbPath = Path.ChangeExtension(AssemblyFilePath, "pdb");
		if (File.Exists(pdbPath))
		{
		    pdbFound = true;
            debugReaderProvider = new PdbReaderProvider();
            debugWriterProvider = new PdbWriterProvider();
			Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", pdbPath));
		}

        mdbPath = AssemblyFilePath + ".mdb";
		if (File.Exists(mdbPath))
		{
		    mdbFound = true;
		    debugReaderProvider = new MdbReaderProvider();
            debugWriterProvider = new MdbWriterProvider();
		    Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", mdbPath));
            return;
		}

		Logger.LogInfo("Found no debug symbols.");
	}

}