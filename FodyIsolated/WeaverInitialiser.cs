using System.IO;
using System.Xml.Linq;
using Fody;
using Mono.Cecil.Cil;

public partial class InnerWeaver
{
    public virtual void SetProperties(WeaverEntry weaverEntry, BaseModuleWeaver weaverInstance)
    {
        if (weaverEntry.Element != null)
        {
            var weaverElement = XElement.Parse(weaverEntry.Element);
            weaverInstance.Config= weaverElement;
        }

        weaverInstance.ModuleDefinition = ModuleDefinition;
        weaverInstance.AssemblyFilePath = AssemblyFilePath;
        weaverInstance.AddinDirectoryPath = Path.GetDirectoryName(weaverEntry.AssemblyPath);
        weaverInstance.References = References;
        weaverInstance.ReferenceCopyLocalPaths = ReferenceCopyLocalPaths;
        weaverInstance.SolutionDirectoryPath = SolutionDirectoryPath;
        weaverInstance.ProjectDirectoryPath = ProjectDirectoryPath;
        weaverInstance.DocumentationFilePath = DocumentationFilePath;
        weaverInstance.LogDebug = message => Logger.LogDebug("  " + message);
        weaverInstance.LogInfo = message => Logger.LogInfo("  " + message);
        weaverInstance.LogMessage = (message, importance) => Logger.LogMessage("  " + message, (int)importance);
        weaverInstance.LogWarning = Logger.LogWarning;
        weaverInstance.LogWarningPoint = LogWarningPoint;
        weaverInstance.LogError = Logger.LogError;
        weaverInstance.LogErrorPoint = LogErrorPoint;
        weaverInstance.DefineConstants = DefineConstants;
        weaverInstance.FindType = TypeCache.FindType;
        weaverInstance.TryFindType = TypeCache.TryFindType;
        weaverInstance.ResolveAssembly = assemblyName => assemblyResolver.Resolve(assemblyName);
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