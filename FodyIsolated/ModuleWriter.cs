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
        Logger.LogInfo(string.Format("Saving assembly to '{0}'.", assemblyPath));

        var parameters = new WriterParameters
            {
                StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = true,
                SymbolWriterProvider = debugWriterProvider,
            };
        ModuleDefinition.Write(assemblyPath, parameters);
    }

}