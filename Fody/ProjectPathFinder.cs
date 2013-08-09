using System.IO;

public partial class Processor
{
    public void ValidateProjectPath()
    {
		if (!Directory.Exists(ProjectDirectory))
        {
			throw new WeavingException(string.Format("ProjectDirectory \"{0}\" does not exist.", ProjectDirectory));
        }
        Logger.LogInfo(string.Format("ProjectDirectory: '{0}'.", ProjectDirectory));
    }

}