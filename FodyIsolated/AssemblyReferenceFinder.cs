using System;
using System.Collections.Generic;
using System.IO;

public class AssemblyReferenceFinder 
{
    InnerWeaver innerWeaver;
    ILogger logger;
    public Dictionary<string, string> References;

    public AssemblyReferenceFinder(InnerWeaver innerWeaver, ILogger logger)
    {
        this.innerWeaver = innerWeaver;
        this.logger = logger;
    }

    public void Execute()
    {
        References = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        SetRefDictionary(innerWeaver.References.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));
        logger.LogInfo("Reference count=" + References.Count);
    }

    void SetRefDictionary(IEnumerable<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            References[Path.GetFileNameWithoutExtension(filePath)] = filePath;
        }
    }

}