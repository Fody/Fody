using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Moq;
using Xunit;

public class WeavingInfoTests : TestBase, IDisposable
{
    string tempFilePath;
    string tempPdbFilePath;

    public WeavingInfoTests()
    {
        var assemblyFilePath = $@"{AssemblyLocation.CurrentDirectory}\DummyAssembly.dll";
        var pdbFilePath = $@"{AssemblyLocation.CurrentDirectory}\DummyAssembly.pdb";
        tempFilePath = $@"{AssemblyLocation.CurrentDirectory}\Temp.dll";
        tempPdbFilePath = $@"{AssemblyLocation.CurrentDirectory}\Temp.pdb";
        File.Copy(assemblyFilePath, tempFilePath, true);
        File.Copy(pdbFilePath, tempPdbFilePath, true);
    }

    [Fact]
    public void WeavedAssembly_ShouldContainWeavedInfo()
    {
        using (var innerWeaver = new InnerWeaver
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
                    AssemblyPath = $@"{AssemblyLocation.CurrentDirectory}\Tests.dll"
                }
            }
        })
        {
            innerWeaver.Execute();
        }

        using (var readModule = ModuleDefinition.ReadModule(tempFilePath))
        {
            var type = readModule.Types
                .Single(_ => _.Name == "ProcessedByFody");
            var condition = type.Fields.Any(f => f.Name == "FodyIsolatedTests");
            Assert.True(condition);
        }
    }

    public void Dispose()
    {
        File.Delete(tempFilePath);
        File.Delete(tempPdbFilePath);
    }
}