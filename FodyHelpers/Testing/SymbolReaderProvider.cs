using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

class SymbolReaderProvider : ISymbolReaderProvider
{
    DefaultSymbolReaderProvider inner;

    public SymbolReaderProvider()
    {
        inner = new DefaultSymbolReaderProvider(false);
    }

    public ISymbolReader? GetSymbolReader(ModuleDefinition module, string fileName)
    {
        var symbolReader = inner.GetSymbolReader(module, fileName);
        if (symbolReader != null)
        {
            return symbolReader;
        }

        var uwpAssemblyPath = Path.ChangeExtension(fileName, "compile.dll");
        return inner.GetSymbolReader(module, uwpAssemblyPath);
    }

    public ISymbolReader? GetSymbolReader(ModuleDefinition module, Stream symbolStream)
    {
        throw new NotSupportedException();
    }
}
