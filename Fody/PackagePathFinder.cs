using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Linq;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class PackagePathFinder
{
    Logger logger;
    SolutionPathFinder solutionPathFinder;
    public string PackagesPath;

    [ImportingConstructor]
    public PackagePathFinder(Logger logger, SolutionPathFinder solutionPathFinder)
    {
        this.logger = logger;
        this.solutionPathFinder = solutionPathFinder;
    }

    string GetPackagesPath()
    {
        var solutionDirectory = solutionPathFinder.SolutionDirectory;
        var nugetConfig = Path.Combine(solutionDirectory, "nuget.config");
        if (File.Exists(nugetConfig))
        {
            var xElement = XDocument.Load(nugetConfig).Root;
            if (xElement != null)
            {
                var element = xElement.Element("repositoryPath");
                if (element != null)
                {
                    var value = element.Value;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return Path.Combine(solutionDirectory, value);
                    }
                }
            }
        }
        return Path.Combine(solutionDirectory, "Packages");
    }

    public void Execute()
    {
        PackagesPath = GetPackagesPath();
        logger.LogInfo(string.Format("\tPackages path is '{0}.'", PackagesPath));
    }
}