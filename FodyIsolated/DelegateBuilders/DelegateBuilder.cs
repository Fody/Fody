using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class DelegateBuilder
{

    public static WeaverDelegate GetDelegateHolderFromCache(this Type weaverType)
    {
        WeaverDelegate @delegate;
        if (!weaverDelegates.TryGetValue(weaverType.TypeHandle, out @delegate))
        {
            weaverDelegates[weaverType.TypeHandle] = @delegate = BuildDelegateHolder(weaverType);
        }
        return @delegate;
    }

    static Dictionary<RuntimeTypeHandle, WeaverDelegate> weaverDelegates = new Dictionary<RuntimeTypeHandle, WeaverDelegate>();

    public static WeaverDelegate BuildDelegateHolder(this  Type weaverType)
    {
        Action<object, ModuleDefinition> moduleDefinitionDelegate; 
        if (!weaverType.TryBuildPropertySetDelegate("ModuleDefinition", out moduleDefinitionDelegate))
        {
            var message = string.Format("{0} must contain a public instance settable property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'.", weaverType.FullName);
            throw new WeavingException(message);
        }

        return new WeaverDelegate
            {
                Execute = weaverType.BuildExecuteDelegate(),
                Cancel = weaverType.BuildCancelDelegate(),
                AfterWeavingExecute = weaverType.BuildAfterWeavingDelegate(),
                SetModuleDefinition = moduleDefinitionDelegate,
                SetConfig = weaverType.BuildPropertySetDelegate<XElement>("Config"),
                SetAddinDirectoryPath = weaverType.BuildPropertySetDelegate<string>("AddinDirectoryPath"),
                SetAssemblyFilePath = weaverType.BuildPropertySetDelegate<string>("AssemblyFilePath"),
                SetAssemblyResolver = weaverType.BuildPropertySetDelegate<IAssemblyResolver>("AssemblyResolver"),
                SetLogError = weaverType.BuildPropertySetDelegate<Action<string>>("LogError"),
                SetLogErrorPoint = weaverType.BuildPropertySetDelegate<Action<string, SequencePoint>>("LogErrorPoint"),
                SetLogDebug = weaverType.BuildPropertySetDelegate<Action<string>>("LogDebug"),
                SetLogInfo = weaverType.BuildPropertySetDelegate<Action<string>>("LogInfo"),
                SetLogMessage = weaverType.BuildPropertySetDelegate<Action<string, MessageImportance>>("LogMessage"),
                SetLogWarning = weaverType.BuildPropertySetDelegate<Action<string>>("LogWarning"),
                SetLogWarningPoint = weaverType.BuildPropertySetDelegate<Action<string, SequencePoint>>("LogWarningPoint"),
                SetReferences = weaverType.BuildPropertySetDelegate<string>("References"),
                SetReferenceCopyLocalPaths = weaverType.BuildPropertySetDelegate<List<string>>("ReferenceCopyLocalPaths"),
                SetSolutionDirectoryPath = weaverType.BuildPropertySetDelegate<string>("SolutionDirectoryPath"),
                SetProjectDirectoryPath = weaverType.BuildPropertySetDelegate<string>("ProjectDirectoryPath"),
                SetDefineConstants = weaverType.BuildPropertySetDelegate<List<string>>("DefineConstants"),
                ConstructInstance = weaverType.BuildConstructorDelegate()
            };
    }

}
