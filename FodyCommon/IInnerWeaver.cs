using System.Collections.Generic;

public interface IInnerWeaver
{
    string AssemblyFilePath { get; set; }
    string References { get; set; }
    string KeyFilePath { get; set; }
    bool SignAssembly { get; set; }
    List<WeaverEntry> Weavers { get; set; }
    ILogger Logger { get; set; }
    string IntermediateDirectoryPath { get; set; }
    string SolutionDirectoryPath { get; set; }
    bool DebugLoggingEnabled { get; set; }
    List<string> ReferenceCopyLocalPaths { get; set; }
    List<string> DefineConstants { get; set; }

    void Execute();
}