using System.IO;
using Mono.Cecil;

public partial class InnerWeaver
{
	public ModuleDefinition ModuleDefinition;

	bool GetSymbolReaderProvider(string assemblyPath)
	{
		var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
		if (File.Exists(pdbPath))
		{
			Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", pdbPath));
			return true;
		}
		var mdbPath = assemblyPath + ".mdb";

		if (File.Exists(mdbPath))
		{
			Logger.LogInfo(string.Format("Found debug symbols at '{0}'.", mdbPath));
			return true;
		}

		Logger.LogInfo("Found no debug symbols.");
		return false;
	}

	public void ReadModule()
	{
		var readSymbols = GetSymbolReaderProvider(AssemblyFilePath);
		var readerParameters = new ReaderParameters
			{
				AssemblyResolver = this,
				ReadSymbols = readSymbols,
			};
		ModuleDefinition = ModuleDefinition.ReadModule(AssemblyFilePath, readerParameters);
	}
}