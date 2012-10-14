using System.IO;

public partial class Processor
{
    public void ValidateProjectPath()
    {
        if (!File.Exists(ProjectFilePath))
        {
            throw new WeavingException(string.Format("ProjectPath \"{0}\" does not exist.", ProjectFilePath));
        }
        Logger.LogInfo(string.Format("ProjectFilePath path is '{0}.'", ProjectFilePath));
    }

}