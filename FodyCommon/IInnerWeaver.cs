using System.Collections.Generic;

public interface IInnerWeaver
{
    string AssemblyFilePath { get; set; }
    string References { get; set; }
    string KeyFilePath { get; set; }
    List<WeaverEntry> Weavers { get; set; }
    ILogger Logger { get; set; }
    string IntermediateDirectoryPath { get; set; }
    string SolutionDirectoryPath { get; set; }
    
    void Execute();
}