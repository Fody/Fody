using System;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class WeaverInitialiserTests
{
    [Test]
    public void ValidProps()
    {
        var moduleWeaver = new ValidModuleWeaver();
        var moduleDefinition = ModuleDefinition.CreateModule("Foo", ModuleKind.Dll);
        var assemblyResolver = Substitute.For<IAssemblyResolver>();
        var innerWeavingTask = new InnerWeaver
                                   {
                                       AssemblyPath = "AssemblyPath"
                                   };

        var moduleWeaverRunner = new WeaverInitialiser
                                     {
                                         ModuleDefinition = moduleDefinition,
                                         Logger = Substitute.For<ILogger>(),
                                         AssemblyResolver = assemblyResolver,
                                         InnerWeavingTask = innerWeavingTask
                                     };
        var weaverEntry = new WeaverEntry {Element = "<foo/>"};
        moduleWeaverRunner.SetProperties( weaverEntry, moduleWeaver);
        
        Assert.IsNotNull(moduleWeaver.LogInfo);
        Assert.IsNotNull(moduleWeaver.LogWarning);
        Assert.IsNotNull(moduleWeaver.LogWarningPoint);
        Assert.IsNotNull(moduleWeaver.LogError);
        Assert.IsNotNull(moduleWeaver.LogErrorPoint);
        Assert.AreEqual(moduleDefinition, moduleWeaver.ModuleDefinition);
        Assert.AreEqual(assemblyResolver, moduleWeaver.AssemblyResolver);
        Assert.AreEqual("AssemblyPath", moduleWeaver.AssemblyPath);
    }

    
    public class ValidModuleWeaver
    {
        public XElement Config { get; set; }
        public string AssemblyPath { get; set; }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public Action<string, SequencePoint> LogWarningPoint { get; set; }
        public Action<string> LogError { get; set; }
        public Action<string, SequencePoint> LogErrorPoint { get; set; }
        public IAssemblyResolver AssemblyResolver { get; set; }
        public ModuleDefinition ModuleDefinition;
        public bool ExecuteCalled;

        public void Execute()
        {
            ExecuteCalled = true;
        }
    }

    
}