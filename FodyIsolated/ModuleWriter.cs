using System.Diagnostics;
using Mono.Cecil;

public partial class InnerWeaver
{
    public virtual void WriteModule()
    {
        var assemblyPath = AssemblyFilePath;
        var stopwatch = Stopwatch.StartNew();
        Logger.LogDebug(string.Format("  Writing assembly to '{0}'.", assemblyPath));

        var parameters = new WriterParameters
            {
                StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = true,
                SymbolWriterProvider = debugWriterProvider,
            };
        ModuleDefinition.Write(assemblyPath, parameters);
        Logger.LogDebug(string.Format("  Finished writing assembly {0}ms.", stopwatch.ElapsedMilliseconds));
    }

}