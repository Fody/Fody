using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModuleWeaver
{
    // Will contain the full element XML from FodyWeavers.xml. OPTIONAL
    public XElement Config { get; set; }

    // Will log an MessageImportance.Normal message to MSBuild. OPTIONAL
    public Action<string> LogDebug { get; set; }

    // Will log an MessageImportance.High message to MSBuild. OPTIONAL
    public Action<string> LogInfo  { get; set; }

    // Will log a message to MSBuild. OPTIONAL
    public Action<string, MessageImportance> LogMessage  { get; set; }

    // Will log an warning message to MSBuild. OPTIONAL
    public Action<string> LogWarning  { get; set; }

    // Will log an warning message to MSBuild at a specific point in the code. OPTIONAL
    public Action<string, SequencePoint> LogWarningPoint { get; set; }

    // Will log an error message to MSBuild. OPTIONAL
    public Action<string> LogError { get; set; }

    // Will log an error message to MSBuild at a specific point in the code. OPTIONAL
    public Action<string, SequencePoint> LogErrorPoint { get; set; }

    // An instance of Mono.Cecil.IAssemblyResolver for resolving assembly references. OPTIONAL
    public IAssemblyResolver AssemblyResolver { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing. REQUIRED
    public ModuleDefinition ModuleDefinition { get; set; }

    // Will contain the full path of the target assembly. OPTIONAL
    public string AssemblyFilePath { get; set; }

    // Will contain the full directory path of the target project. 
    // A copy of $(ProjectDir). OPTIONAL
    public string ProjectDirectoryPath { get; set; }

    // Will contain the full directory path of the current weaver. OPTIONAL
    public string AddinDirectoryPath { get; set; }

    // Will contain the full directory path of the current solution.
    // A copy of `$(SolutionDir)` or, if it does not exist, a copy of `$(MSBuildProjectDirectory)..\..\..\`. OPTIONAL
    public string SolutionDirectoryPath { get; set; }

    // Will contain a semicolon delimited string that contains 
    // all the references for the target project. 
    // A copy of the contents of the @(ReferencePath). OPTIONAL
    public string References { get; set; }

    // Will a list of all the references marked as copy-local. 
    // A copy of the contents of the @(ReferenceCopyLocalPaths). OPTIONAL
    public List<string> ReferenceCopyLocalPaths { get; set; }

    // Will a list of all the msbuild constants. 
    // A copy of the contents of the $(DefineConstants). OPTIONAL
    public List<string> DefineConstants { get; set; }

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogDebug = m => { };
        LogInfo = m => { };
        LogWarning = m => { };
        LogWarningPoint = (m,p) => { };
        LogError = m => { };
        LogErrorPoint = (m, p) => { };
    } 

    public void Execute()
    {
        ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public));
    }

    // Will be called when a request to cancel the build occurs. OPTIONAL
    public void Cancel()
    {
    }

    // Will be called after all weaving has occurred and the module has been saved. OPTIONAL
    public void AfterWeaving()
    {
    }
}