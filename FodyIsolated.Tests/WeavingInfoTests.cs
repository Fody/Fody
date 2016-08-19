using System.Collections.Generic;
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
        var moduleDefinition =
            ModuleDefinition.ReadModule(TestContext.CurrentContext.TestDirectory + "\\DummyAssembly.dll");

        var innerWeaver = new InnerWeaver
        {
            References = string.Empty,
            Logger = new Mock<ILogger>().Object,
            AssemblyFilePath = TestContext.CurrentContext.TestDirectory + "\\DummyAssembly.dll",
            ModuleDefinition = moduleDefinition,
            DefineConstants = new List<string> { "Debug", "Release" },
            Weavers = new List<WeaverEntry> {
                new WeaverEntry
                {
                    TypeName = "ModuleWeaver",
                    AssemblyName = "Sample.Fody",
                    AssemblyPath = TestContext.CurrentContext.TestDirectory + "\\Sample.Fody.dll"
                }
            }
        };

        innerWeaver.Execute();

        moduleDefinition =
            ModuleDefinition.ReadModule(TestContext.CurrentContext.TestDirectory + "\\DummyAssembly.dll");

        Assert.IsTrue(moduleDefinition.Types.Count(_ => _.Name == "FodyWeavingResults") >= 1);
        Assert.IsTrue(moduleDefinition.Types.First(_ => _.Name == "FodyWeavingResults").HasCustomAttributes);
        Assert.IsTrue(moduleDefinition.Types.First(_ => _.Name == "FodyWeavingResults").Fields.Any(f => f.Name == "SampleFody"));
    }
}
