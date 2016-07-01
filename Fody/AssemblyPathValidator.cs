public partial class Processor
{

    public virtual void ValidateAssemblyPath()
    {
        AssemblyFilePath = FileSystem.Path.Combine(ProjectDirectory, AssemblyFilePath);
        if (!FileSystem.File.Exists(AssemblyFilePath))
        {
            throw new WeavingException($"AssemblyPath \"{AssemblyFilePath}\" does not exists. If you have not done a build you can ignore this error.");
        }

        Logger.LogDebug($"AssemblyPath: '{AssemblyFilePath}'");
    }

}