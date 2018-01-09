using System.Diagnostics;
using System.Linq;
using Mono.Cecil;

public partial class InnerWeaver
{
    public virtual void WriteModule()
    {
        if (weaverInstances.All(weaver => weaver.WeaverDelegate.GetIsAssemblyUnmodified(weaver.Instance)))
        {
            Logger.LogDebug("  No weaver modified the assembly, skipping write");
            return;
        }
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

        ModuleDefinition.Write(AssemblyFilePath, parameters);
        Logger.LogDebug($"  Finished writing assembly {stopwatch.ElapsedMilliseconds}ms.");
    }
}