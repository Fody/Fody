using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class InnerWeaver : MarshalByRefObject, IInnerWeaver
{


    public static WeaverDelegate GetDelegateHolderFromCache(Type weaverType)
    {
        WeaverDelegate @delegate;
        if (!weaverDelegates.TryGetValue(weaverType.TypeHandle, out @delegate))
        {
            weaverDelegates[weaverType.TypeHandle] = @delegate = BuildDelegateHolder(weaverType);
        }
        return @delegate;
    }

    static Dictionary<RuntimeTypeHandle, WeaverDelegate> weaverDelegates = new Dictionary<RuntimeTypeHandle, WeaverDelegate>();

    public static WeaverDelegate BuildDelegateHolder(Type weaverType)
    {
        var delegateHolder = new WeaverDelegate();
        var moduleDefinitionProperty = weaverType.GetPropertySetMethod<ModuleDefinition>("ModuleDefinition");
        if (moduleDefinitionProperty == null)
        {
            var message = string.Format("{0} must contain a public instance settable property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'.", weaverType.FullName);
            throw new WeavingException(message);
        }

        delegateHolder.Execute = weaverType.BuildExecuteDelegate();
        delegateHolder.SetModuleDefinition = weaverType.BuildPropertySetDelegate<ModuleDefinition>("ModuleDefinition");
        delegateHolder.SetConfig = weaverType.BuildPropertySetDelegate<XElement>("Config");
        delegateHolder.SetAddinDirectoryPath = weaverType.BuildPropertySetDelegate<string>("AddinDirectoryPath");
        delegateHolder.SetAssemblyFilePath = weaverType.BuildPropertySetDelegate<string>("AssemblyFilePath");
        delegateHolder.SetAssemblyResolver = weaverType.BuildPropertySetDelegate<IAssemblyResolver>("AssemblyResolver");
        delegateHolder.SetLogError = weaverType.BuildPropertySetDelegate<Action<string>>("LogError");
        delegateHolder.SetLogErrorPoint = weaverType.BuildPropertySetDelegate<Action<string, SequencePoint>>("LogErrorPoint");
        delegateHolder.SetLogInfo = weaverType.BuildPropertySetDelegate<Action<string>>("LogInfo");
        delegateHolder.SetLogWarning = weaverType.BuildPropertySetDelegate<Action<string>>("LogWarning");
        delegateHolder.SetLogWarningPoint = weaverType.BuildPropertySetDelegate<Action<string, SequencePoint>>("LogWarningPoint");
        delegateHolder.SetReferenceCopyLocalPaths = weaverType.BuildPropertySetDelegate<List<string>>("ReferenceCopyLocalPaths");
        delegateHolder.SetSolutionDirectoryPath = weaverType.BuildPropertySetDelegate<string>("SolutionDirectoryPath");
        delegateHolder.SetDefineConstants = weaverType.BuildPropertySetDelegate<string>("DefineConstants");
        delegateHolder.ConstructInstance = weaverType.BuildConstructorDelegate();
        return delegateHolder;
    }

    void LogWarningPoint(string message, SequencePoint point)
    {
        if (point == null)
        {
            Logger.LogWarning(message);
        }
        else
        {
            Logger.LogWarning(message, point.Document.Url, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn);
        }
    }

    void LogErrorPoint(string message, SequencePoint point)
    {
        if (point == null)
        {
            Logger.LogError(message);
        }
        else
        {
            Logger.LogError(message, point.Document.Url, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn);
        }
    }
}
