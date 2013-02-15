using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class InnerWeaver
{

    static Dictionary<RuntimeTypeHandle, WeaverDelegateHolder> weaverDelegates = new Dictionary<RuntimeTypeHandle, WeaverDelegateHolder>();

    public static WeaverDelegateHolder BuildDelegateHolder(Type weaverType)
    {
        var delegateHolder = new WeaverDelegateHolder();
        var moduleDefinitionProperty = weaverType.GetProperty<ModuleDefinition>("ModuleDefinition");
        if (moduleDefinitionProperty == null)
        {
            var message = string.Format("{0} must contain a public instance settable property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'.", weaverType.FullName);
            throw new WeavingException(message);
        }

        var executeMethod = weaverType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public, null, new Type[] {}, null);
        if (executeMethod == null)
        {
            var message = string.Format("{0} must contain a public instance method named 'Execute'.", weaverType.FullName);
            throw new WeavingException(message);
        }

        delegateHolder.Execute = o => executeMethod.Invoke(o, null);
        delegateHolder.SetModuleDefinition = weaverType.BuildPropertyGetter<ModuleDefinition>(moduleDefinitionProperty);
        delegateHolder.SetConfig = weaverType.BuildPropertyGetter<XElement>("SetConfig");
        delegateHolder.SetAddinDirectoryPath = weaverType.BuildPropertyGetter<string>("SetAddinDirectoryPath");
        delegateHolder.SetAssemblyFilePath = weaverType.BuildPropertyGetter<string>("SetAssemblyFilePath");
        delegateHolder.SetAssemblyResolver = weaverType.BuildPropertyGetter<IAssemblyResolver>("SetAssemblyResolver");
        delegateHolder.SetLogError = weaverType.BuildPropertyGetter<Action<string>>("SetLogError");
        delegateHolder.SetLogErrorPoint = weaverType.BuildPropertyGetter<Action<string, SequencePoint>>("SetLogErrorPoint");
        delegateHolder.SetLogInfo = weaverType.BuildPropertyGetter<Action<string>>("SetLogInfo");
        delegateHolder.SetLogWarning = weaverType.BuildPropertyGetter<Action<string>>("SetLogWarning");
        delegateHolder.SetLogWarningPoint = weaverType.BuildPropertyGetter<Action<string, SequencePoint>>("SetLogWarningPoint");
        delegateHolder.SetReferenceCopyLocalPaths = weaverType.BuildPropertyGetter<List<string>>("SetReferenceCopyLocalPaths");
        delegateHolder.SetSolutionDirectoryPath = weaverType.BuildPropertyGetter<string>("SetSolutionDirectoryPath");
        delegateHolder.SetSolutionDirectoryPath = weaverType.BuildPropertyGetter<string>("SetSolutionDirectoryPath");
        return delegateHolder;
    }

    public void SetProperties(WeaverEntry weaverEntry, object weaverInstance, WeaverDelegateHolder delegateHolder)
    {
        if (weaverEntry.Element != null)
        {
            var weaverElement = XElement.Parse(weaverEntry.Element);
            delegateHolder.SetConfig(weaverInstance, weaverElement);
        }
        delegateHolder.SetModuleDefinition(weaverInstance, ModuleDefinition);
        delegateHolder.SetAssemblyResolver(weaverInstance, this);
        delegateHolder.SetAssemblyFilePath(weaverInstance, AssemblyFilePath);
        delegateHolder.SetAddinDirectoryPath(weaverInstance, Path.GetDirectoryName(weaverEntry.AssemblyPath));
        delegateHolder.SetReferenceCopyLocalPaths(weaverInstance, ReferenceCopyLocalPaths);
        delegateHolder.SetSolutionDirectoryPath(weaverInstance, SolutionDirectoryPath);
        delegateHolder.SetLogWarning(weaverInstance, Logger.LogWarning);
        delegateHolder.SetLogWarningPoint(weaverInstance, LogWarningPoint);
        delegateHolder.SetLogInfo(weaverInstance, Logger.LogInfo);
        delegateHolder.SetLogError(weaverInstance, Logger.LogInfo);
        delegateHolder.SetLogErrorPoint(weaverInstance, LogErrorPoint);
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