using System.IO;

public class NugetDirectoryFinder
{
    public NugetPackagePathFinder NugetPackagePathFinder;
    public AddinDirectories AddinDirectories;
    public ILogger Logger;

    public void Execute()
    {
        if (Directory.Exists(NugetPackagePathFinder.PackagesPath))
        {
            AddinDirectories.SearchPaths.Add(NugetPackagePathFinder.PackagesPath);
        }
        else
        {
            Logger.LogInfo(string.Format("Could not search for addins in '{0}' because it does not exist", NugetPackagePathFinder.PackagesPath));
        }
    }

}