using System;
using System.Collections.Generic;
using Fody;
using Mono.Cecil;

public static class DelegateBuilder
{
    public static bool InheritsFromBaseWeaver(this Type weaverType)
    {
        return typeof(BaseModuleWeaver).IsAssignableFrom(weaverType);
    }

    public static WeaverDelegate GetDelegateHolderFromCache(this Type weaverType)
    {
        if (!weaverDelegates.TryGetValue(weaverType.TypeHandle, out var @delegate))
        {
            weaverDelegates[weaverType.TypeHandle] = @delegate = BuildDelegateHolder(weaverType);
        }
        return @delegate;
    }

    static Dictionary<RuntimeTypeHandle, WeaverDelegate> weaverDelegates = new Dictionary<RuntimeTypeHandle, WeaverDelegate>();

    public static WeaverDelegate BuildDelegateHolder(this Type weaverType)
    {
        if (!weaverType.TryBuildPropertySetDelegate("ModuleDefinition", out Action<object, ModuleDefinition> moduleDefinitionDelegate))
        {
            var message = $"Cannot load/use weaver {weaverType.FullName}. Note that the weaver must contain a public instance settable property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'. If it does, make sure that it's referencing the right version of Mono.Cecil, which is '{typeof (ModuleDefinition).Assembly.GetName().Version}'.";
            throw new WeavingException(message);
        }

        return new WeaverDelegate
        {
            Execute = weaverType.BuildExecuteDelegate(),
            Cancel = weaverType.BuildCancelDelegate(),
            AfterWeavingExecute = weaverType.BuildAfterWeavingDelegate(),
            SetModuleDefinition = moduleDefinitionDelegate,
            SetConfig = weaverType.BuildSetConfig(),
            SetAddinDirectoryPath = weaverType.BuildSetAddinDirectoryPath(),
            SetAssemblyFilePath = weaverType.BuildSetAssemblyFilePath(),
            SetAssemblyResolver = weaverType.BuildSetAssemblyResolver(),
            SetResolveAssembly = weaverType.BuildSetResolveAssembly(),
            SetLogError = weaverType.BuildSetLogError(),
            SetLogErrorPoint = weaverType.BuildSetLogErrorPoint(),
            SetLogDebug = weaverType.BuildSetLogDebug(),
            SetLogInfo = weaverType.BuildSetLogInfo(),
            SetLogMessage = weaverType.BuildSetLogMessage(),
            SetLogWarning = weaverType.BuildSetLogWarning(),
            SetLogWarningPoint = weaverType.BuildSetLogWarningPoint(),
            SetReferences = weaverType.BuildSetReferences(),
            SetReferenceCopyLocalPaths = weaverType.BuildSetReferenceCopyLocalPaths(),
            SetSolutionDirectoryPath = weaverType.BuildSetSolutionDirectoryPath(),
            SetProjectDirectoryPath = weaverType.BuildSetProjectDirectoryPath(),
            SetDocumentationFilePath = weaverType.BuildSetDocumentationFilePath(),
            SetDefineConstants = weaverType.BuildSetDefineConstants(),
            SetFindType = weaverType.BuildSetFindType(),
            GetIsAssemblyUnmodified = weaverType.BuildIsAssemblyUnmodifiedDelegate(),
            GetAssembliesForScanning = weaverType.BuildGetAssembliesForScanningDelegate(),
            ConstructInstance = weaverType.BuildConstructorDelegate()
        };
    }
}