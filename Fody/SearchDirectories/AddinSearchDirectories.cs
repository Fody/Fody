using System.Collections.Generic;
using System.Linq;

public partial class Processor
{
    public List<string> AddinSearchPaths= new List<string>();

    public virtual void LogAddinSearchPaths()
    {
        AddinSearchPaths = AddinSearchPaths.Distinct().ToList();
        foreach (var searchPath in AddinSearchPaths)
        {
            Logger.LogInfo(string.Format("Directory added to addin search paths '{0}'.", searchPath));
        }
    }

}