using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using WithEmbeddedPdb;
using Xunit;

public class WithEmbeddedPdbTest
{
    [Fact]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.True(typeof(Class1).GetMethod("Method").IsVirtual);
    }

    [Fact]
    public void EnsureDebugInfoIsPresent()
    {
        var filePath = typeof(Class1).Assembly.Location;

        using (var file = File.OpenRead(filePath))
        using (var peReader = new PEReader(file))
        {
            var debugInfo = peReader.ReadDebugDirectory();
            Assert.Contains(debugInfo, _ => _.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);

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
                    Assert.EndsWith("Class1.cs", docName);
                    found = true;
                    break;
                }

                Assert.True(found);
            }
        }
    }
}