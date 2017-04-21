using System.IO;

public partial class Processor
{
    public virtual void ValidateProjectPath()
    {
        if (!Directory.Exists(ProjectDirectory))
        {
            throw new WeavingException($"ProjectDirectory \"{ProjectDirectory}\" does not exist.");
        }
        Logger.LogDebug($"ProjectDirectory: '{ProjectDirectory}'.");
    }

}