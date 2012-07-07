using System.IO;
using Fody;

public class AssemblyPathValidator
{
    public WeavingTask WeavingTask;
    public BuildLogger Logger;

    public void Execute()
    {
        if (!File.Exists(WeavingTask.AssemblyPath))
        {
            throw new WeavingException(string.Format("AssemblyPath \"{0}\" does not exists. If you have not done a build you can ignore this error.", WeavingTask.AssemblyPath));
        }

        Logger.LogInfo(string.Format("AssemblyPath: {0}", WeavingTask.AssemblyPath));
    }

}