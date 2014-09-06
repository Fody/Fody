using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

public static class NugetConfigReader
{

    public static string GetPackagesPathFromConfig(string currentDirectory)
    {
        while (true)
        {
            var packagePath = GetPackagePath(Path.Combine(currentDirectory, "nuget.config"));
            if (packagePath != null)
            {
                return packagePath;
            }
            packagePath = GetPackagePath(Path.Combine(currentDirectory, ".nuget", "nuget.config"));
            if (packagePath != null)
            {
                return packagePath;
            }
            try
            {
                var directoryInfo = Directory.GetParent(currentDirectory);
                if (directoryInfo == null)
                {
                    return null;
                }
                currentDirectory = directoryInfo.FullName;
            }
            catch 
            {
                // trouble with tree walk. ignore
                return null;
            }
        }
    }

    public static string GetPackagePath(string nugetConfigPath)
    {
        if (File.Exists(nugetConfigPath))
        {
            XDocument xDocument;
            try
            {
                xDocument = XDocument.Load(nugetConfigPath);
            }
            catch (XmlException)
            {
                return null;
            }
            var repositoryPath = xDocument.Descendants("repositoryPath")
                .Select(x => x.Value)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            if (repositoryPath != null)
            {
                return Path.Combine(Path.GetDirectoryName(nugetConfigPath), repositoryPath);
            }
            repositoryPath = xDocument.Descendants("add")
                .Where(x =>   string.Equals((string)x.Attribute("key"), "repositoryPath",StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Attribute("value"))
                .Where(x => x != null)
                .Select(x => x.Value)
                .FirstOrDefault();
            if (repositoryPath != null)
            {
                if (repositoryPath.StartsWith("$\\"))
                {
                    return repositoryPath.Replace("$", Path.Combine(Path.GetDirectoryName(nugetConfigPath)));
                }

                return Path.Combine(Path.GetDirectoryName(nugetConfigPath), repositoryPath);
            }
        }
        return null;
    }
}