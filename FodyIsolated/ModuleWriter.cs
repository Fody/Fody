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
#if NET46
                StrongNameKeyPair = StrongNameKeyPair,
#endif
                WriteSymbols = debugWriterProvider != null,
                SymbolWriterProvider = debugWriterProvider,
            };

        ModuleDefinition.Assembly.Name.PublicKey = PublicKey;
        ModuleDefinition.Write(AssemblyFilePath, parameters);
        Logger.LogDebug($"  Finished writing assembly {stopwatch.ElapsedMilliseconds}ms.");
    }
}