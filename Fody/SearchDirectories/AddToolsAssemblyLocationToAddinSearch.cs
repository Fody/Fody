using System.IO;

public partial class Processor
{

    public void AddToolsAssemblyLocationToAddinSearch()
    {
        var parent = Directory.GetParent(AssemblyLocation.CurrentDirectory()).FullName;
        var assemblyLocationToolsDirectory = Path.GetFullPath(Path.Combine(parent, "Tools"));
        
        if (Directory.Exists(assemblyLocationToolsDirectory))
        {
            AddinSearchPaths.Add(assemblyLocationToolsDirectory);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", assemblyLocationToolsDirectory));
        }
    }
}