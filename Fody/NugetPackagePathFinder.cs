using System.IO;
using System.Xml.Linq;
using Fody;

public class NugetPackagePathFinder
{
    public ILogger Logger;
    public WeavingTask WeavingTask;
    public string PackagesPath;

    string GetPackagesPath()
    {
        var nugetConfigPath = GetNugetConfigPath(WeavingTask.SolutionDir);

        if (nugetConfigPath != null)
        {
            var xElement = XDocument.Load(nugetConfigPath).Root;
            if (xElement != null)
            {
                var element = xElement.Element("repositoryPath");
                if (element != null)
                {
                    var value = element.Value;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return Path.Combine(Path.GetDirectoryName(nugetConfigPath), value);
                    }
                }
            }
        }
        return Path.Combine(WeavingTask.SolutionDir, "Packages");
    }

    static string GetNugetConfigPath(string solutionDirectory)
    {
        while (true)
        {
            var nugetConfigPath = Path.Combine(solutionDirectory, "nuget.config");
            if (File.Exists(nugetConfigPath))
            {
                return nugetConfigPath;
            }
            var directoryInfo = Directory.GetParent(solutionDirectory);
            if (directoryInfo == null)
            {
                return null;
            }
            solutionDirectory = directoryInfo.FullName;
        }
    }

    public virtual void Execute()
    {
        PackagesPath = GetPackagesPath();
        Logger.LogInfo(string.Format("Packages path is '{0}'", PackagesPath));
    }
}