using System.IO;

public partial class Processor
{

    public void ValidateAssemblyPath()
    {
        AssemblyFilePath = Path.Combine(ProjectDirectory, AssemblyFilePath);
        if (!File.Exists(AssemblyFilePath))
        {
            throw new WeavingException(string.Format("AssemblyPath \"{0}\" does not exists. If you have not done a build you can ignore this error.", AssemblyFilePath));
        }

        Logger.LogInfo(string.Format("AssemblyPath: '{0}'", AssemblyFilePath));
    }

}