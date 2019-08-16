using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class WeaverInitialiserTests :
    XunitLoggingBase
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
        ObjectApprover.Verify(moduleWeaver);
    }

    static InnerWeaver BuildInnerWeaver(ModuleDefinition moduleDefinition, AssemblyResolver resolver)
    {
        return new InnerWeaver
        {
            Logger = new MockBuildLogger(),
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

    public WeaverInitialiserTests(ITestOutputHelper output) :
        base(output)
    {
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