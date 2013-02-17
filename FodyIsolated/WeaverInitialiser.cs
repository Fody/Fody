using System.IO;
using System.Xml.Linq;

public partial class InnerWeaver
{

    public void SetProperties(WeaverEntry weaverEntry, object weaverInstance, WeaverDelegate @delegate)
    {
        if (weaverEntry.Element != null)
        {
            var weaverElement = XElement.Parse(weaverEntry.Element);
            @delegate.SetConfig(weaverInstance, weaverElement);
        }
        @delegate.SetModuleDefinition(weaverInstance, ModuleDefinition);
        @delegate.SetAssemblyResolver(weaverInstance, this);
        @delegate.SetAssemblyFilePath(weaverInstance, AssemblyFilePath);
        @delegate.SetAddinDirectoryPath(weaverInstance, Path.GetDirectoryName(weaverEntry.AssemblyPath));
        @delegate.SetReferenceCopyLocalPaths(weaverInstance, ReferenceCopyLocalPaths);
        @delegate.SetSolutionDirectoryPath(weaverInstance, SolutionDirectoryPath);
        @delegate.SetLogWarning(weaverInstance, Logger.LogWarning);
        @delegate.SetLogWarningPoint(weaverInstance, LogWarningPoint);
        @delegate.SetLogInfo(weaverInstance, Logger.LogInfo);
        @delegate.SetLogError(weaverInstance, Logger.LogInfo);
        @delegate.SetLogErrorPoint(weaverInstance, LogErrorPoint);
        @delegate.SetDefineConstants(weaverInstance, DefineConstants);
    }


}