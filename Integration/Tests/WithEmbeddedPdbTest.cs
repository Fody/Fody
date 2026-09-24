using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using WithEmbeddedPdb;

public class WithEmbeddedPdbTest
{
    [Test]
    public async Task EnsureTypeChangedByNugetWeaver()
    {
        await Assert.That(typeof(Class1).GetMethod("Method").IsVirtual).IsTrue();
    }

    [Test]
    public async Task EnsureDebugInfoIsPresent()
    {
        var filePath = typeof(Class1).Assembly.Location;

        using (var file = File.OpenRead(filePath))
        using (var peReader = new PEReader(file))
        {
            var debugInfo = peReader.ReadDebugDirectory();
            await Assert.That(debugInfo.Any(_ => _.Type == DebugDirectoryEntryType.EmbeddedPortablePdb)).IsTrue();

            var metadataReader = peReader.GetMetadataReader();

            using (var provider = peReader.ReadEmbeddedPortablePdbDebugDirectoryData(debugInfo.Single(_ => _.Type == DebugDirectoryEntryType.EmbeddedPortablePdb)))
            {
                var debugReader = provider.GetMetadataReader();

                var found = false;

                foreach (var debugInfoHandle in debugReader.MethodDebugInformation)
                {
                    var method = metadataReader.GetMethodDefinition(debugInfoHandle.ToDefinitionHandle());
                    if (metadataReader.GetString(method.Name) != "Method")
                        continue;

                    var methodDebugInfo = debugReader.GetMethodDebugInformation(debugInfoHandle);
                    var docHandle = debugReader.GetDocument(methodDebugInfo.Document);

                    var docName = debugReader.GetString(docHandle.Name);
                    await Assert.That(docName).EndsWith("Class1.cs");
                    found = true;
                    break;
                }

                await Assert.That(found).IsTrue();
            }
        }
    }
}