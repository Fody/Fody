using System.Diagnostics;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using TypeAttributes = Mono.Cecil.TypeAttributes;

public partial class InnerWeaver
{

    static ISymbolWriterProvider GetSymbolWriterProvider(string assemblyPath)
    {
        var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
        if (File.Exists(pdbPath))
        {
            return new PdbWriterProvider();
        }
        var mdbPath = Path.ChangeExtension(assemblyPath, "mdb");

        if (File.Exists(mdbPath))
        {
            return new MdbWriterProvider();
        }
        return null;
    }

    public void WriteModule()
    {
        ModuleDefinition.Types.Add(new TypeDefinition(null, "ProcessedByFody", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        var assemblyPath = AssemblyFilePath;
        Logger.LogInfo(string.Format("Saving assembly to '{0}'.", assemblyPath));
        var parameters = new WriterParameters
                             {
                                 StrongNameKeyPair = StrongNameKeyPair,
                                 WriteSymbols = true,
                                 SymbolWriterProvider = GetSymbolWriterProvider(assemblyPath)
                             };
        var startNew = Stopwatch.StartNew();
        try
        {
            ModuleDefinition.Write(assemblyPath, parameters);
        }
        finally
        {
            startNew.Stop();
            Logger.LogInfo(string.Format("Finished Saving {0}ms.", startNew.ElapsedMilliseconds));
        }
    }

}