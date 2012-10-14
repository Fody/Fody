using System.IO;

public partial class AddinFinder
{
    public string PackagesPath;

    string GetPackagesPath()
    {
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(SolutionDirectoryPath);
        if (packagesPathFromConfig!= null)
        {
            return packagesPathFromConfig;
        }
        return Path.Combine(SolutionDirectoryPath, "Packages");
    }


    public void FindNugetPackagePath()
    {
        PackagesPath = GetPackagesPath();
        Logger.LogInfo(string.Format("Packages path is '{0}'", PackagesPath));
    }
}