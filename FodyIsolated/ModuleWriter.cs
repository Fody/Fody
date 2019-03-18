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
#pragma warning disable 618
                StrongNameKeyPair = StrongNameKeyPair,
#pragma warning restore 618
                WriteSymbols = debugWriterProvider != null,
                SymbolWriterProvider = debugWriterProvider,
            };

        //TODO: cecil should handle the patching
        //ModuleDefinition.Assembly.Name.PublicKey = PublicKey;
        ModuleDefinition.Write(AssemblyFilePath, parameters);
        Logger.LogDebug($"  Finished writing assembly {stopwatch.ElapsedMilliseconds}ms.");
    }
}