using System;
using System.Linq;
using Mono.Cecil;

public class ContainsTypeChecker
{
    public virtual bool Check(string assemblyPath, string typeName)
    {
        var parameters = new ReaderParameters
        {
            ReadWrite = false,
            ReadingMode = ReadingMode.Deferred,
            ReadSymbols = false,
        };
        using (var module = ModuleDefinition.ReadModule(assemblyPath, parameters))
        {
            var types = module.Types;
            return types.Any(x => string.Equals(x.Name, typeName, StringComparison.OrdinalIgnoreCase));
        }
    }
}