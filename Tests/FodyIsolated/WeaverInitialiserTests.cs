using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Moq;
using Xunit;

public class WeaverInitialiserTests : TestBase
{
    [Fact]
    public void ValidProps()
    {
        var moduleDefinition = ModuleDefinition.CreateModule("Foo", ModuleKind.Dll);

        var resolver = new MockAssemblyResolver();
        var innerWeaver = new InnerWeaver
        {
            Logger = new Mock<ILogger>().Object,
            AssemblyFilePath = "AssemblyFilePath",
            ProjectDirectoryPath = "ProjectDirectoryPath",
            SolutionDirectoryPath = "SolutionDirectoryPath",
            DocumentationFilePath = "DocumentationFilePath",
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
            assemblyResolver = resolver
        };
        innerWeaver.SplitUpReferences();

        var weaverEntry = new WeaverEntry
        {
            Element = "<foo/>",
            AssemblyPath = @"c:\FakePath\Assembly.dll"
        };
        var moduleWeaver = new ValidModuleWeaver();
        innerWeaver.SetProperties(weaverEntry, moduleWeaver, typeof(ValidModuleWeaver).BuildDelegateHolder());

        Assert.NotNull(moduleWeaver.LogDebug);
        Assert.NotNull(moduleWeaver.LogInfo);
        Assert.NotNull(moduleWeaver.LogWarning);
        Assert.NotNull(moduleWeaver.LogWarningPoint);
        Assert.NotNull(moduleWeaver.LogError);
        Assert.NotNull(moduleWeaver.LogErrorPoint);
        Assert.NotNull(moduleWeaver.LogMessage);
        Assert.Equal("Ref1;Ref2", moduleWeaver.References);
        Assert.Equal("CopyRef1", moduleWeaver.ReferenceCopyLocalPaths[0]);
        Assert.Equal("CopyRef2", moduleWeaver.ReferenceCopyLocalPaths[1]);
        Assert.Equal("Debug", moduleWeaver.DefineConstants[0]);
        Assert.Equal("Release", moduleWeaver.DefineConstants[1]);

        // Assert.IsNotEmpty(moduleWeaver.References);
        Assert.Equal(moduleDefinition, moduleWeaver.ModuleDefinition);
        Assert.Equal(resolver, moduleWeaver.AssemblyResolver);
        Assert.Equal(@"c:\FakePath", moduleWeaver.AddinDirectoryPath);
        Assert.Equal("AssemblyFilePath", moduleWeaver.AssemblyFilePath);
        Assert.Equal("ProjectDirectoryPath", moduleWeaver.ProjectDirectoryPath);
        Assert.Equal("SolutionDirectoryPath", moduleWeaver.SolutionDirectoryPath);
        Assert.Equal("DocumentationFilePath", moduleWeaver.DocumentationFilePath);
    }
}

public class ValidModuleWeaver
{
    public XElement Config { get; set; }

    //   public List<string> References { get; set; }
    public string AssemblyFilePath { get; set; }

    public string ProjectDirectoryPath { get; set; }
    public string DocumentationFilePath { get; set; }
    public string AddinDirectoryPath { get; set; }
    public Action<string> LogDebug { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string, SequencePoint> LogWarningPoint { get; set; }
    public Action<string> LogError { get; set; }
    public Action<string, SequencePoint> LogErrorPoint { get; set; }
    public Action<string, MessageImportance> LogMessage { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public List<string> DefineConstants { get; set; }

    public string References { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }

    public bool ExecuteCalled;

    public void Execute()
    {
        ExecuteCalled = true;
    }
}