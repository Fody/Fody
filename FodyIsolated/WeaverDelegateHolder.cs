using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class WeaverDelegateHolder
{
    public Action<object> Execute;
    public Action<object, XElement> SetConfig;
    public Action<object, ModuleDefinition> SetModuleDefinition;
    public Action<object, IAssemblyResolver> SetAssemblyResolver;
    public Action<object, string> SetAssemblyFilePath;
    public Action<object, string> SetAddinDirectoryPath;
    public Action<object, List<string>> SetReferenceCopyLocalPaths;
    public Action<object, string> SetSolutionDirectoryPath;
    public Action<object, Action<string>> SetLogInfo;
    public Action<object, Action<string>> SetLogError;
    public Action<object, Action<string, SequencePoint>> SetLogErrorPoint;
    public Action<object, Action<string>> SetLogWarning;
    public Action<object, Action<string, SequencePoint>> SetLogWarningPoint;
    public Action<object, string> SetDefineConstants;

	public Func<object> ConstructInstance;
}