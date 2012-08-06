using System.IO;
using System.Xml.Linq;

public static class NugetConfigReader
{

    public static string GetPackagesPathFromConfig(string solutionDir)
    {
        var nugetConfigPath = GetNugetConfigPath(solutionDir);

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
        return null;
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
                // trouble with tree walk. ignore
                return null;
            }
        }
    }

}