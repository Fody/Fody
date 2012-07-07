using System;
using System.Xml.Linq;
using Mono.Cecil;
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
        moduleWeaverRunner.SetProperties( new WeaverEntry {Element = "<foo/>"}, moduleWeaver);
        
        Assert.IsNotNull(moduleWeaver.LogInfo);
        Assert.IsNotNull(moduleWeaver.LogWarning);
        Assert.AreEqual(moduleDefinition, moduleWeaver.ModuleDefinition);
        Assert.AreEqual(assemblyResolver, moduleWeaver.AssemblyResolver);
        Assert.AreEqual("AssemblyPath", moduleWeaver.AssemblyPath);
    }

    
    public class ValidModuleWeaver
    {
        public XElement Config { get; set; }
        public string AssemblyPath { get; set; }
        public Action<string> LogInfo = s => { };
        public Action<string> LogWarning = s => { };
        public IAssemblyResolver AssemblyResolver { get; set; }
        public ModuleDefinition ModuleDefinition;
        public bool ExecuteCalled;

        public void Execute()
        {
            ExecuteCalled = true;
        }
    }

    
}