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
            Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", pdbPath));
		}

        mdbPath = AssemblyFilePath + ".mdb";
		if (File.Exists(mdbPath))
		{
            mdbFound = true;
            Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", mdbPath));
		}

        if (pdbFound && mdbFound)
        {
            if (File.GetLastWriteTimeUtc(pdbPath) >= File.GetLastWriteTimeUtc(mdbPath))
            {
                mdbFound = false;
                Logger.LogInfo("Found mdb and pdb debug symbols. Selected pdb (newer).");
            }
            else
            {
                pdbFound = false;
                Logger.LogInfo("Found mdb and pdb debug symbols. Selected mdb (newer).");
            }
        }

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


		Logger.LogInfo("Found no debug symbols.");
	}

}