using System.Diagnostics;
using MethodTimer;
using Mono.Cecil;

public partial class InnerWeaver
{
    [Time]
    public void WriteModule()
    {
        var assemblyPath = AssemblyFilePath;
        var stopwatch = Stopwatch.StartNew();
        Logger.LogInfo(string.Format("  Writing assembly to '{0}'.", assemblyPath));

        var parameters = new WriterParameters
            {
                StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = true,
                SymbolWriterProvider = debugWriterProvider,
            };
        ModuleDefinition.Write(assemblyPath, parameters);
        Logger.LogInfo(string.Format(" Finished writing assembly {0}ms.", stopwatch.ElapsedMilliseconds));
    }

}