using System.IO;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using NUnit.Framework;

[TestFixture]
[Explicit]
public class AdHocCecilTests
{
    [Test]
    public void Foo()
    {
        ModuleDefinition moduleDefinition;
        using (var symbolStream = File.OpenRead(@"C:\Code\containsdynamiclocals.pdb"))
        {
            var readerParameters = new ReaderParameters
            {
                ReadSymbols = true,
                SymbolReaderProvider =  new PdbReaderProvider(),
                SymbolStream = symbolStream
            };
            moduleDefinition = ModuleDefinition.ReadModule(@"C:\Code\containsdynamiclocals.dll", readerParameters);
        }
        File.Delete(@"C:\Code\containsdynamiclocals2.dll");
        File.Delete(@"C:\Code\containsdynamiclocals2.pdb");

        var parameters = new WriterParameters
        {
            WriteSymbols = true,
            SymbolWriterProvider = new PdbWriterProvider(),
        };
        moduleDefinition.Write(@"C:\Code\containsdynamiclocals2.dll", parameters);

    }
}