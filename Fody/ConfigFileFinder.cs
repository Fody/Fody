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
        if (File.Exists(projectConfigFilePath))
        {
            files.Add(projectConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{projectConfigFilePath}'.");
        }


        if (files.Count == 0)
        {
            var pathsSearched = string.Join("', '", solutionConfigFilePath, projectConfigFilePath);
            logger.LogDebug($"Could not find path to weavers file. Searched '{pathsSearched}'.");
        }
        return files;
    }

}