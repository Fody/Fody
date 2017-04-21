using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Moq;
using NUnit.Framework;

[TestFixture]
public class WeavingInfoTests
{
    [Test]
    public void WeavedAssembly_ShouldContainWeavedInfo()
    {
        var assemblyFilePath = $@"{TestContext.CurrentContext.TestDirectory}\DummyAssembly.dll";
        var pdbFilePath = $@"{TestContext.CurrentContext.TestDirectory}\DummyAssembly.pdb";
        var tempFilePath = $@"{TestContext.CurrentContext.TestDirectory}\Temp.dll";
        var tempPdbFilePath = $@"{TestContext.CurrentContext.TestDirectory}\Temp.pdb";
        File.Copy(assemblyFilePath, tempFilePath, true);
        File.Copy(pdbFilePath, tempPdbFilePath, true);
        var innerWeaver = new InnerWeaver
        {
            References = string.Empty,
            Logger = new Mock<ILogger>().Object,
            AssemblyFilePath = tempFilePath,
            DefineConstants = new List<string>
            {
                "Debug",
                "Release"
            },
            Weavers = new List<WeaverEntry>
            {
                new WeaverEntry
                {
                    TypeName = "FakeModuleWeaver",
                    AssemblyName = "FodyIsolated.Tests",
                    AssemblyPath = TestContext.CurrentContext.TestDirectory + "\\FodyIsolated.Tests.dll"
                }
            }
        };
        innerWeaver.Execute();

        using (var readModule = ModuleDefinition.ReadModule(tempFilePath))
        {
            var type = readModule.Types
                .Single(_ => _.Name == "ProcessedByFody");
            Assert.IsTrue(type.Fields.Any(f => f.Name == "FodyIsolatedTests"));
        }
    }
}