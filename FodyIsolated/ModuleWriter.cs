using System.Diagnostics;
using Mono.Cecil;

public partial class InnerWeaver
{
    public virtual void WriteModule()
    {
        var stopwatch = Stopwatch.StartNew();
        Logger.LogDebug($"  Writing assembly to '{AssemblyFilePath}'.");

        var parameters = new WriterParameters
            {
                StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = true,
                SymbolWriterProvider = debugWriterProvider,
            };

        ModuleDefinition.Write(AssemblyFilePath, parameters);
        Logger.LogDebug($"  Finished writing assembly {stopwatch.ElapsedMilliseconds}ms.");
    }
}