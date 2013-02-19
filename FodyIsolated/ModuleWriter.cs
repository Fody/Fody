using System.Diagnostics;
using MethodTimer;
using Mono.Cecil;
using TypeAttributes = Mono.Cecil.TypeAttributes;

public partial class InnerWeaver
{
    [Time]
    public void WriteModule()
    {
        ModuleDefinition.Types.Add(new TypeDefinition(null, "ProcessedByFody", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        var assemblyPath = AssemblyFilePath;
        var stopwatch = Stopwatch.StartNew();
        Logger.LogInfo(string.Format("\tWriting assembly to '{0}'.", assemblyPath));

        var parameters = new WriterParameters
            {
                StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = true,
                SymbolWriterProvider = debugWriterProvider,
            };
        ModuleDefinition.Write(assemblyPath, parameters);
        Logger.LogInfo(string.Format("\tFinished writing assembly {0}ms.", stopwatch.ElapsedMilliseconds));
    }

}