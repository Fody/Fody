using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class WeaverInitialiser
{
    public ModuleDefinition ModuleDefinition;
    public ILogger Logger;
    public IAssemblyResolver AssemblyResolver;
    public InnerWeaver InnerWeavingTask;
    public List<dynamic> WeaverInstances = new List<dynamic>();


    public void Execute()
    {
        foreach (var weaverConfig in InnerWeavingTask.Weavers)
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
            SetConfig(weaverInstance, weaverElement);
        }
        SetModule(weaverInstance);
        SetAssemblyResolver(weaverInstance);
        SetAssemblyPath(weaverInstance);
        SetLogInfo(weaverInstance);
        SetLogWarning(weaverInstance);
        SetLogWarningPoint(weaverInstance);
        SetLogError(weaverInstance);
        SetLogErrorPoint(weaverInstance);
    }

    void SetModule(dynamic instance)
    {
        try
        {
            instance.ModuleDefinition = ModuleDefinition;
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                throw new WeavingException(string.Format("{0} must contain a public property named 'ModuleDefinition' of type 'Mono.Cecil.ModuleDefinition'.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'ModuleDefinition' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'ModuleDefinition' of '{0}' has an incorrect type. Expected 'Mono.Cecil.ModuleDefinition'.", ((object) instance).GetTypeName()));
            }
            throw;
        }
        catch (RuntimeBinderInternalCompilerException)
        {
            var instanceAsObject = (object) instance;
            var propertyInfo = instanceAsObject.GetType().GetProperty("ModuleDefinition");
            if (propertyInfo == null)
            {
                throw;
            }
            var targetPropType = propertyInfo.PropertyType;
            var actualModuleType = ModuleDefinition.GetType();
            if (targetPropType != actualModuleType)
            {
                throw new WeavingException(string.Format("{0} should contain a 'ModuleDefinition' property of type '{1}', but instead contains one of type '{2}'.", instanceAsObject.GetTypeName(), actualModuleType.AssemblyQualifiedName, targetPropType.AssemblyQualifiedName));
            }
            throw;
        }
    }

    void SetAssemblyResolver(dynamic instance)
    {
        try
        {
            instance.AssemblyResolver = AssemblyResolver;
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'AssemblyResolver' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'AssemblyResolver' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'AssemblyResolver' of '{0}' has an incorrect type. Expected 'Mono.Cecil.IAssemblyResolver'.", ((object) instance).GetTypeName()));
            }
            throw;
        }
    }

    void SetLogWarning(dynamic instance)
    {
        try
        {
            instance.LogWarning = new Action<string>(s => Logger.LogWarning(s));
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogWarning' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogWarning' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'LogWarning' of '{0}' has an incorrect type. Expected 'Action<string>'.", ((object) instance).GetTypeName()));
            }
            throw;
        }
    }
    
    void SetLogWarningPoint(dynamic instance)
    {
        try
        {
            instance.LogWarningPoint = new Action<string, SequencePoint>(LogWarningPoint);
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogWarningPoint' of '{0}' must not be static.", ((object)instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogWarningPoint' of '{0}' must not be static.", ((object)instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'LogWarningPoint' of '{0}' has an incorrect type. Expected 'Action<string, SequencePoint>'.", ((object)instance).GetTypeName()));
            }
            throw;
        }
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

    void SetLogError(dynamic instance)
    {
        try
        {
            instance.LogError = new Action<string>(s => Logger.LogError(s));
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogError' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogError' of '{0}' must not be static.", ((object)instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'LogError' of '{0}' has an incorrect type. Expected 'Action<string>'.", ((object)instance).GetTypeName()));
            }
            throw;
        }
    }

    void SetLogErrorPoint(dynamic instance)
    {
        try
        {
            instance.LogErrorPoint = new Action<string, SequencePoint>(LogErrorPoint);
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'SetLogErrorPoint' of '{0}' must not be static.", ((object)instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'SetLogErrorPoint' of '{0}' must not be static.", ((object)instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'SetLogErrorPoint' of '{0}' has an incorrect type. Expected 'Action<string, SequencePoint>'.", ((object)instance).GetTypeName()));
            }
            throw;
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

    void SetAssemblyPath(dynamic instance)
    {
        try
        {
            instance.AssemblyPath = InnerWeavingTask.AssemblyPath;
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'AssemblyPath' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'AssemblyPath' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'AssemblyPath' of '{0}' has an incorrect type. Expected 'string'.", ((object) instance).GetTypeName()));
            }
            throw;
        }
    }

    void SetLogInfo(dynamic instance)
    {
        try
        {
            instance.LogInfo = new Action<string>(s => Logger.LogInfo(s));
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogInfo' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'LogInfo' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'LogInfo' of '{0}' has an incorrect type. Expected 'Action<string>'.", ((object) instance).GetTypeName()));
            }
            throw;
        }
    }

    void SetConfig(dynamic instance, XElement weaverConfig)
    {
        try
        {
            instance.Config = weaverConfig;
        }
        catch (RuntimeBinderException exception)
        {
            if (exception.IsNoDefinition())
            {
                return;
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'Config' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsStatic())
            {
                throw new WeavingException(string.Format("Property 'Config' of '{0}' must not be static.", ((object) instance).GetTypeName()));
            }
            if (exception.IsWrongType())
            {
                throw new WeavingException(string.Format("Property 'Config' of '{0}' has an incorrect type. Expected 'XElement'.", ((object) instance).GetTypeName()));
            }
        }
    }

}