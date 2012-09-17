using System.IO;

public partial class AddinFinder
{

    public void AddNugetDirectoryToAddinSearch()
    {
        if (Directory.Exists(PackagesPath))
        {
            AddinSearchPaths.Add(PackagesPath);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", PackagesPath));
        }
    }

}