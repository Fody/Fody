using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class WeaverInitialiser
{
    public ModuleDefinition ModuleDefinition;
    public ILogger Logger;
    public IAssemblyResolver AssemblyResolver;
    public InnerWeaver InnerWeaver;
    public List<dynamic> WeaverInstances = new List<dynamic>();


    public void Execute()
    {
        foreach (var weaverConfig in InnerWeaver.Weavers)
        {
            SetProperties(weaverConfig);
        }
    }

    public void SetProperties(WeaverEntry weaverEntry)
    {
        Logger.LogInfo(string.Format("Loading weaver '{0}'.", weaverEntry.AssemblyPath));
        var assembly = AssemblyLoader.Load(weaverEntry.AssemblyPath);
        
        var weaverType = assembly.FindType(weaverEntry.TypeName);
        var weaverInstance = weaverType.ConstructInstance();

        SetProperties(weaverEntry, weaverInstance);
        WeaverInstances.Add(weaverInstance);
    }

    public void SetProperties(WeaverEntry weaverEntry, object weaverInstance)
    {
        if (weaverEntry.Element != null)
        {
            var weaverElement = XElement.Parse(weaverEntry.Element);
            weaverInstance.SetProperty("Config",weaverElement);
        }
        var type = weaverInstance.GetType();
        SetModule(weaverInstance, type);
        weaverInstance.SetProperty("AssemblyResolver", AssemblyResolver);
        weaverInstance.SetProperty("AssemblyPath", InnerWeaver.AssemblyPath);
        weaverInstance.SetProperty("LogInfo", new Action<string>(s => Logger.LogInfo(s)));
        weaverInstance.SetProperty("LogWarning", new Action<string>(s => Logger.LogWarning(s)));
        weaverInstance.SetProperty("LogWarningPoint", new Action<string, SequencePoint>(LogWarningPoint));
        weaverInstance.SetProperty("LogError", new Action<string>(s => Logger.LogError(s)));
        weaverInstance.SetProperty("LogErrorPoint", new Action<string, SequencePoint>(LogErrorPoint));
    }

    void SetModule(object instance, Type type)
    {
        var property = type.GetProperty<ModuleDefinition>("ModuleDefinition");
        if (property == null)
        {
            var message = string.Format("{0} must contain a public instance settable property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'.", type.GetTypeName());
            throw new WeavingException(message);
        }
        property.SetValue(instance, ModuleDefinition, null);
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