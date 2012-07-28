using System.Collections.Generic;

public interface IInnerWeaver
{
    string AssemblyPath { get; set; }
    string References { get; set; }
    string KeyFilePath { get; set; }
    List<WeaverEntry> Weavers { get; set; }
    ILogger Logger { get; set; }
    string IntermediateDir { get; set; }
    void Execute();
}