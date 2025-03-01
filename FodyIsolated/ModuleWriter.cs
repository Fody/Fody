using System.Diagnostics;

public partial class InnerWeaver
{
    public virtual void WriteModule()
    {
        var stopwatch = Stopwatch.StartNew();
        Logger.LogDebug($"Writing assembly to '{AssemblyFilePath}'.");

        var parameters = new WriterParameters
        {
            StrongNameKeyBlob = StrongNameKeyBlob,
            WriteSymbols = hasSymbols
        };

        ModuleDefinition.Assembly.Name.PublicKey = PublicKey;
        ModuleDefinition.Write(AssemblyFilePath, parameters);
        Logger.LogDebug($"Finished writing assembly {stopwatch.ElapsedMilliseconds}ms.");
    }
}
