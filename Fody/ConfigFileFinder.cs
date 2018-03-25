using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fody;

public class ConfigFileFinder
{
    private static void AddIfFileExists(List<string> configFiles, string configFilePath, ILogger logger, bool logNotFound = false)
    {
        if (File.Exists(configFilePath))
        {
            configFiles.Add(configFilePath);
            logger.LogDebug($"Found path to weavers file '{configFilePath}'.");
        }
        else if (logNotFound)
        {
            logger.LogWarning($"Unable to find weavers file '{configFilePath}'.");
        }
    }
    public static List<string> FindWeaverConfigs(string projectDirectory, ILogger logger, IEnumerable<string> directoriesToSearchIn = null, IEnumerable<string> configFilePaths = null)
    {
        var files = new List<string>();

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

        if (directoriesToSearchIn != null)
        {
            foreach (var directory in directoriesToSearchIn)
            {
                var configFilePath = Path.Combine(directory, "FodyWeavers.xml");
                AddIfFileExists(files, configFilePath, logger);
            }
        }

        if (configFilePaths != null)
        {
            foreach (var configFilePath in configFilePaths)
            {
                AddIfFileExists(files, configFilePath, logger, true);
            }
        }

        if (files.Count == 0)
        {
            IEnumerable<string> pathsSearched = new[] { projectConfigFilePath };
            if (directoriesToSearchIn != null)
                pathsSearched = pathsSearched.Union(directoriesToSearchIn);
            if (configFilePaths != null)
                pathsSearched = pathsSearched.Union(configFilePaths);
            var pathsSearchedString = string.Join("', '", pathsSearched);
            throw new WeavingException($"Could not find path to weavers file. Searched '{pathsSearchedString}'.");
        }
        return files;
    }

    public static List<string> FindWeaverConfigs(string solutionDirectoryPath, string projectDirectory, ILogger logger) =>
        FindWeaverConfigs(projectDirectory, logger, new[] { solutionDirectoryPath }, null);
}