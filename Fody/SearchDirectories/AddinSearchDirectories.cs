using System.Collections.Generic;

public partial class Processor
{
    public List<string> AddinSearchPaths= new List<string>();

    public virtual void LogAddinSearchPaths()
    {
        foreach (var searchPath in AddinSearchPaths)
        {
            Logger.LogInfo(string.Format("Directory added to addin search paths '{0}'.", searchPath));
        }
    }

}