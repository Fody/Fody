using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Moq;
using ObjectApproval;
using Xunit;

public class WeaverInitialiserTests : TestBase
{
    [Fact]
    public void ValidPropsFromBase()
    {
        var moduleDefinition = ModuleDefinition.CreateModule("Foo", ModuleKind.Dll);

        var resolver = new MockAssemblyResolver();
        var innerWeaver = BuildInnerWeaver(moduleDefinition, resolver);
        innerWeaver.SplitUpReferences();

        var weaverEntry = new WeaverEntry
        {
            Element = "<foo/>",
            AssemblyPath = @"c:\FakePath\Assembly.dll"
        };
        var moduleWeaver = new ValidFromBaseModuleWeaver();
        innerWeaver.SetProperties(weaverEntry, moduleWeaver);

        SerializerBuilder.IgnoreMembersWithType<ModuleDefinition>();
        ObjectApprover.VerifyWithJson(moduleWeaver,
            x=>x
                .Replace("<SetProperties>b__99_0","SetProperties")
                .Replace("<SetProperties>b__100_0","SetProperties")
                .Replace("<SetProperties>b__99_1","SetProperties")
                .Replace("<SetProperties>b__100_1","SetProperties")
                .Replace("<SetProperties>b__99_2","SetProperties")
                .Replace("<SetProperties>b__100_2","SetProperties")
                .Replace("<SetProperties>b__99_3","SetProperties")
                .Replace("<SetProperties>b__100_3","SetProperties"));
    }

    static InnerWeaver BuildInnerWeaver(ModuleDefinition moduleDefinition, AssemblyResolver resolver)
    {
#pragma warning disable 618
        return new InnerWeaver
        {
            Logger = new Mock<ILogger>().Object,
            AssemblyFilePath = "AssemblyFilePath",
            ProjectDirectoryPath = "ProjectDirectoryPath",
            ProjectFilePath = "ProjectFilePath",
            SolutionDirectoryPath = "SolutionDirectoryPath",
            DocumentationFilePath = "DocumentationFile",
            ReferenceCopyLocalPaths = new List<string>
            {
                "CopyRef1",
                "CopyRef2"
            },
            References = "Ref1;Ref2",
            ModuleDefinition = moduleDefinition,
            DefineConstants = new List<string>
            {
                "Debug",
                "Release"
            },
            assemblyResolver = resolver,
            TypeCache = new TypeCache(resolver.Resolve)
        };
    }
}

public class ValidFromBaseModuleWeaver : BaseModuleWeaver
{
    public bool ExecuteCalled;

    public override void Execute()
    {
        ExecuteCalled = true;
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }
}