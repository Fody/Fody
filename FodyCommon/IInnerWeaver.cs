using System.Collections.Generic;
using System;

public interface IInnerWeaver : IDisposable
{
    string AssemblyFilePath { get; set; }
    string References { get; set; }
    string? KeyFilePath { get; set; }
    bool SignAssembly { get; set; }
    bool DelaySign { get; set; }
    List<WeaverEntry> Weavers { get; set; }
    ILogger Logger { get; set; }
    string IntermediateDirectoryPath { get; set; }
    string SolutionDirectoryPath { get; set; }
    List<string> ReferenceCopyLocalPaths { get; set; }
    List<string> DefineConstants { get; set; }
    string ProjectDirectoryPath { get; set; }
    string ProjectFilePath { get; set; }
    string? DocumentationFilePath { get; set; }
    #if(NETSTANDARD)
    IsolatedAssemblyLoadContext LoadContext { get; set; }
    #endif
    void Execute();
    void Cancel();
}
