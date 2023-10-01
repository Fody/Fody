using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using Mono.Cecil;
using VerifyTests;
using VerifyXunit;

[UsesVerify]
public class WeaverInitialiserTests
{
    [Fact]
    public Task ValidPropsFromBase()
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
        var verifySettings = new VerifySettings();
        verifySettings.IgnoreMembersWithType<ModuleDefinition>();
        verifySettings.IncludeObsoletes();
        verifySettings.UniqueForRuntime();

        return VerifyXunit.Verifier.Verify(moduleWeaver, verifySettings);
    }

    static InnerWeaver BuildInnerWeaver(ModuleDefinition moduleDefinition, IAssemblyResolver resolver) =>
        new()
        {
            Logger = new MockBuildLogger(),
            AssemblyFilePath = "AssemblyFilePath",
            ProjectDirectoryPath = "ProjectDirectoryPath",
            ProjectFilePath = "ProjectFilePath",
            SolutionDirectoryPath = "SolutionDirectoryPath",
            DocumentationFilePath = "DocumentationFile",
            ReferenceCopyLocalPaths = new()
            {
                "CopyRef1",
                "CopyRef2"
            },
            References = "Ref1;Ref2",
            ModuleDefinition = moduleDefinition,
            DefineConstants = new()
            {
                "Debug",
                "Release"
            },
            assemblyResolver = resolver,
            TypeCache = new(resolver.Resolve)
        };
}

public class ValidFromBaseModuleWeaver : BaseModuleWeaver
{
    public bool ExecuteCalled;

    public override void Execute() =>
        ExecuteCalled = true;

    public override IEnumerable<string> GetAssembliesForScanning() =>
        Enumerable.Empty<string>();
}