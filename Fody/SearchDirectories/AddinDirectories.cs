using System.Collections.Generic;

public class AddinDirectories
{
    public ILogger Logger;
    public List<string> SearchPaths= new List<string>();

    public virtual void Execute()
    {
        foreach (var searchPath in SearchPaths)
        {
            Logger.LogInfo(string.Format("Directory added to addin search paths '{0}'.", searchPath));
        }
    }

}