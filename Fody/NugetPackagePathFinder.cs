using System.IO;
using System.Xml.Linq;

public partial class Processor
{
    public string PackagesPath;

    string GetPackagesPath()
    {
        var nugetConfigPath = GetNugetConfigPath(SolutionDir);

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
        return Path.Combine(SolutionDir, "Packages");
    }

    string GetNugetConfigPath(string solutionDirectory)
    {
        while (true)
        {
            var nugetConfigPath = Path.Combine(solutionDirectory, "nuget.config");
            if (File.Exists(nugetConfigPath))
            {
                return nugetConfigPath;
            }
            try
            {
                var directoryInfo = Directory.GetParent(solutionDirectory);
                if (directoryInfo == null)
                {
                    return null;
                }
                solutionDirectory = directoryInfo.FullName;
            }
            catch 
            {
                // trouble with tree walk. log and ignore
                Logger.LogInfo(string.Format("Could not get parent directory of '{0}'.", solutionDirectory));
                return null;
            }
        }
    }

    public virtual void FindNugetPackagePath()
    {
        PackagesPath = GetPackagesPath();
        Logger.LogInfo(string.Format("Packages path is '{0}'", PackagesPath));
    }
}