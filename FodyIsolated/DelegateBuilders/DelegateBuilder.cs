using System;
using System.Collections.Generic;
using System.Reflection;
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

    public static WeaverDelegate BuildDelegateHolder(this Type weaverType)
    {
        Action<object, ModuleDefinition> moduleDefinitionDelegate;
        if (!weaverType.TryBuildPropertySetDelegate("ModuleDefinition", out moduleDefinitionDelegate))
        {
            var message = string.Format("Cannot load/use weaver {0}. Note that the weaver must contain a public instance settable property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'. If it does, make sure that it's referencing the right version of Mono.Cecil, which is '{1}'.", weaverType.FullName, GetAssemblyVersion(typeof(ModuleDefinition).Assembly));
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

    private static string GetAssemblyVersion(Assembly assembly, int separatorCount = 3)
    {
        separatorCount++;

        // Get full name, which is in [name], Version=[version], Culture=[culture], PublicKeyToken=[publickeytoken] format
        string assemblyFullName = assembly.FullName;

        string[] splittedAssemblyFullName = assemblyFullName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (splittedAssemblyFullName.Length < 2)
        {
            return "unknown";
        }

        string version = splittedAssemblyFullName[1].Replace("Version=", string.Empty).Trim();
        string[] versionSplit = version.Split('.');
        version = versionSplit[0];
        for (int i = 1; i < separatorCount; i++)
        {
            if (i >= versionSplit.Length)
            {
                break;
            }

            version += string.Format(".{0}", versionSplit[i]);
        }

        return version;
    }

}
