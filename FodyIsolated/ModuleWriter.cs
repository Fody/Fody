using System.Diagnostics;
using Mono.Cecil;

public partial class InnerWeaver
{
    public virtual void WriteModule()
    {
        var assemblyPath = AssemblyFilePath;
        var stopwatch = Stopwatch.StartNew();
        Logger.LogDebug($"  Writing assembly to '{assemblyPath}'.");

        var parameters = new WriterParameters
            {
                StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = true,
                SymbolWriterProvider = debugWriterProvider,
            };
        ModuleDefinition.Write(assemblyPath, parameters);
        Logger.LogDebug($"  Finished writing assembly {stopwatch.ElapsedMilliseconds}ms.");
    }

}