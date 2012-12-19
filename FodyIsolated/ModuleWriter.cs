using System.Diagnostics;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using TypeAttributes = Mono.Cecil.TypeAttributes;

public partial class InnerWeaver
{


    public void WriteModule()
    {
        ModuleDefinition.Types.Add(new TypeDefinition(null, "ProcessedByFody", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        var assemblyPath = AssemblyFilePath;
        Logger.LogInfo(string.Format("Saving assembly to '{0}'.", assemblyPath));

        var parameters = new WriterParameters
                             {
                                 StrongNameKeyPair = StrongNameKeyPair,
                                 WriteSymbols = true,
                                 SymbolWriterProvider = debugWriterProvider,
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