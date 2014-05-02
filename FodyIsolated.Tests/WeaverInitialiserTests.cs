using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaverInitialiserTests
{
	[Test]
	public void ValidProps()
	{
		var moduleDefinition = ModuleDefinition.CreateModule("Foo", ModuleKind.Dll);

		var innerWeaver = new InnerWeaver
			{
				Logger = new Mock<ILogger>().Object,
				AssemblyFilePath = "AssemblyFilePath",
				ProjectDirectoryPath = "ProjectDirectoryPath",
				SolutionDirectoryPath = "SolutionDirectoryPath",
				ReferenceDictionary = new Dictionary<string, string> {{"Ref1;Ref2", "Path1"}},
				ReferenceCopyLocalPaths = new List<string> {"Ref1"},
				ModuleDefinition = moduleDefinition,
                DefineConstants = new List<string>{"Debug", "Release"}
			};

		var weaverEntry = new WeaverEntry
			{
				Element = "<foo/>",
				AssemblyPath = @"c:\FakePath\Assembly.dll"
			};
		var moduleWeaver = new ValidModuleWeaver();
        innerWeaver.SetProperties(weaverEntry, moduleWeaver, (typeof(ValidModuleWeaver)).BuildDelegateHolder());

		Assert.IsNotNull(moduleWeaver.LogInfo);
		Assert.IsNotNull(moduleWeaver.LogDebug);
		Assert.IsNotNull(moduleWeaver.LogWarning);
		Assert.IsNotNull(moduleWeaver.LogWarningPoint);
		Assert.IsNotNull(moduleWeaver.LogError);
		Assert.IsNotNull(moduleWeaver.LogErrorPoint);
		Assert.AreEqual("Ref1", moduleWeaver.ReferenceCopyLocalPaths.First());
		Assert.AreEqual("Debug", moduleWeaver.DefineConstants[0]);
		Assert.AreEqual("Release", moduleWeaver.DefineConstants[1]);

		// Assert.IsNotEmpty(moduleWeaver.References); 
		Assert.AreEqual(moduleDefinition, moduleWeaver.ModuleDefinition);
		Assert.AreEqual(innerWeaver, moduleWeaver.AssemblyResolver);
		Assert.AreEqual(@"c:\FakePath", moduleWeaver.AddinDirectoryPath);
		Assert.AreEqual("AssemblyFilePath", moduleWeaver.AssemblyFilePath);
        Assert.AreEqual("ProjectDirectoryPath", moduleWeaver.ProjectDirectoryPath);
		Assert.AreEqual("SolutionDirectoryPath", moduleWeaver.SolutionDirectoryPath);
	}

}

public class ValidModuleWeaver
{
	public XElement Config { get; set; }
	//   public List<string> References { get; set; }
	public string AssemblyFilePath { get; set; }
    public string ProjectDirectoryPath { get; set; }
	public string AddinDirectoryPath { get; set; }
	public Action<string> LogInfo { get; set; }
	public Action<string> LogDebug { get; set; }
	public Action<string> LogWarning { get; set; }
	public Action<string, SequencePoint> LogWarningPoint { get; set; }
	public Action<string> LogError { get; set; }
	public Action<string, SequencePoint> LogErrorPoint { get; set; }
	public IAssemblyResolver AssemblyResolver { get; set; }
	public ModuleDefinition ModuleDefinition { get; set; }
	public string SolutionDirectoryPath { get; set; }
    public List<string> DefineConstants { get; set; }

	public List<string> ReferenceCopyLocalPaths { get; set; }

	public bool ExecuteCalled;

	public void Execute()
	{
		ExecuteCalled = true;
	}
}