using System.IO;
using System.Xml.Linq;
using Mono.Cecil.Cil;

public partial class InnerWeaver
{

    public virtual void SetProperties(WeaverEntry weaverEntry, object weaverInstance, WeaverDelegate @delegate)
    {
        if (weaverEntry.Element != null)
        {
            var weaverElement = XElement.Parse(weaverEntry.Element);
            @delegate.SetConfig(weaverInstance, weaverElement);
        }

        @delegate.SetModuleDefinition(weaverInstance, ModuleDefinition);
        @delegate.SetAssemblyResolver(weaverInstance, assemblyResolver);
        @delegate.SetAssemblyFilePath(weaverInstance, AssemblyFilePath);
        @delegate.SetAddinDirectoryPath(weaverInstance, Path.GetDirectoryName(weaverEntry.AssemblyPath));
        @delegate.SetReferences(weaverInstance, References);
        @delegate.SetReferenceCopyLocalPaths(weaverInstance, ReferenceCopyLocalPaths);
        @delegate.SetSolutionDirectoryPath(weaverInstance, SolutionDirectoryPath);
        @delegate.SetProjectDirectoryPath(weaverInstance, ProjectDirectoryPath);
        @delegate.SetLogDebug(weaverInstance, s => Logger.LogDebug("  " + s));
        @delegate.SetLogInfo(weaverInstance, s => Logger.LogInfo("  " + s));
        @delegate.SetLogMessage(weaverInstance, (s, importance) => Logger.LogMessage("  " + s, importance));
        @delegate.SetLogWarning(weaverInstance, Logger.LogWarning);
        @delegate.SetLogWarningPoint(weaverInstance, LogWarningPoint);
        @delegate.SetLogError(weaverInstance, Logger.LogError);
        @delegate.SetLogErrorPoint(weaverInstance, LogErrorPoint);
        @delegate.SetDefineConstants(weaverInstance, DefineConstants);
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