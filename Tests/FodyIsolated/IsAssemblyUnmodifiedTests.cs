using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Moq;
using Xunit;

public class IsAssemblyUnmodifiedTests : TestBase, IDisposable
{
    string tempFilePath;
    string tempPdbFilePath;

    public IsAssemblyUnmodifiedTests()
    {
        var assemblyFilePath = $@"{AssemblyLocation.CurrentDirectory}\DummyAssembly.dll";
        var pdbFilePath = $@"{AssemblyLocation.CurrentDirectory}\DummyAssembly.pdb";
        tempFilePath = $@"{AssemblyLocation.CurrentDirectory}\Temp.dll";
        tempPdbFilePath = $@"{AssemblyLocation.CurrentDirectory}\Temp.pdb";
        File.Copy(assemblyFilePath, tempFilePath, true);
        File.Copy(pdbFilePath, tempPdbFilePath, true);
    }

    [Fact]
    public void WeavedAssembly_ShouldNotBeWrittenIfUnmodified()
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
                    TypeName = nameof(NonModifyingFakeModuleWeaver),
                    AssemblyName = "FodyIsolated.Tests",
                    AssemblyPath = $@"{AssemblyLocation.CurrentDirectory}\Tests.dll"
                }
            },
        })
        {
            innerWeaver.Execute();
        }

        using (var readModule = ModuleDefinition.ReadModule(tempFilePath))
        {
            var type = readModule.Types
                .FirstOrDefault(_ => _.Name == "ProcessedByFody");
            Assert.Null(type);
        }
    }

    public void Dispose()
    {
        File.Delete(tempFilePath);
        File.Delete(tempPdbFilePath);
    }
}