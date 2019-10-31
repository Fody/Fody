using System.IO;

public partial class Processor
{
    public virtual void ValidateAssemblyPath()
    {
        AssemblyFilePath = Path.Combine(ProjectDirectory, AssemblyFilePath);
        if (!File.Exists(AssemblyFilePath))
        {
            throw new WeavingException($"AssemblyFile '{AssemblyFilePath}' does not exists. If you have not done a build you can ignore this error.");
        }

        Logger.LogDebug($"AssemblyFile: '{AssemblyFilePath}'");
    }
}