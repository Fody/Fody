using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class InnerWeaver 
{
    public Dictionary<string, string> ReferenceDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    public List<string> SplitReferences;

    public virtual void SplitUpReferences()
    {
        SplitReferences = References
            .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        SetRefDictionary(SplitReferences);
        Logger.LogInfo("Reference count=" + ReferenceDictionary.Count);
    }


    void SetRefDictionary(IEnumerable<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            ReferenceDictionary[Path.GetFileNameWithoutExtension(filePath)] = filePath;
        }
    }

}