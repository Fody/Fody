using System.Collections.Generic;
using System.IO;

public class ConfigFileFinder
{

    public static List<string> FindWeaverConfigs(string solutionDirectoryPath, string projectDirectory, ILogger logger)
    {
        var files = new List<string>();

        var solutionConfigFilePath = Path.Combine(solutionDirectoryPath, "FodyWeavers.xml");
        if (File.Exists(solutionConfigFilePath))
        {
            files.Add(solutionConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{solutionConfigFilePath}'.");
        }

        var projectConfigFilePath = Path.Combine(projectDirectory, "FodyWeavers.xml");
        if (!File.Exists(projectConfigFilePath))
        {
            logger.LogDebug($@"Could not file a FodyWeavers.xml at the project level ({projectConfigFilePath}). Some project types do not support using NuGet to add content files e.g. netstandard projects. In these cases it is necessary to manually add a FodyWeavers.xml to the project. Example content:
  <Weavers>
    <WeaverName/>
  </Weavers>
  ");
        }
        else
        {
            files.Add(projectConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{projectConfigFilePath}'.");
        }

        if (files.Count == 0)
        {
            var pathsSearched = string.Join("', '", solutionConfigFilePath, projectConfigFilePath);
            throw new WeavingException($"Could not find path to weavers file. Searched '{pathsSearched}'.");
        }
        return files;
    }

}