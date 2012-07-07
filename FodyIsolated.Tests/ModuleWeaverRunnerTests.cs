using System;
using System.Xml.Linq;
using Mono.Cecil;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class ModuleWeaverRunnerTests
{

    [Test]
    public void Execute()
    {
        var moduleWeaver = new ValidModuleWeaver();
        var moduleWeaverRunner = new ModuleWeaverRunner { Logger = Substitute.For<ILogger>() };
        moduleWeaverRunner.Execute(moduleWeaver);
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "ModuleWeaver must contain a method with the signature 'public void Execute()'.")]
    public void BadExecute()
    {
        var moduleWeaver = new BadExecuteWithParamsModuleWeaver();
        var moduleWeaverRunner = new ModuleWeaverRunner { Logger = Substitute.For<ILogger>() };
        moduleWeaverRunner.Execute( moduleWeaver);
    }

    [Test]
    [ExpectedException(typeof (WeavingException))]
    public void NoExecute()
    {
        var moduleWeaverRunner = new ModuleWeaverRunner { Logger = Substitute.For<ILogger>() };
        moduleWeaverRunner.Execute("sdf");
    }

    public class ValidModuleWeaver
    {
        public XElement Config { get; set; }
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

    public class BadExecuteWithParamsModuleWeaver
    {
        public void Execute(string foo)
        {
        }
    }
}