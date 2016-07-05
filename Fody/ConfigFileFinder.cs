using System.Collections.Generic;
using System.IO.Abstractions;

public class ConfigFileFinder
{
    public static List<string> FindWeaverConfigs(string solutionDirectoryPath, string projectDirectory, ILogger logger, IFileSystem fileSystem = null)
    {
        if (fileSystem == null)
            fileSystem = new FileSystem();

        var files = new List<string>();
        var fodyDirConfigFilePath = fileSystem.Path.Combine(AssemblyLocation.CurrentDirectory, "FodyWeavers.xml");
        if (fileSystem.File.Exists(fodyDirConfigFilePath))
        {
            files.Add(fodyDirConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{fodyDirConfigFilePath}'.");
        }

        var solutionConfigFilePath = fileSystem.Path.Combine(solutionDirectoryPath, "FodyWeavers.xml");
        if (fileSystem.File.Exists(solutionConfigFilePath))
        {
            files.Add(solutionConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{solutionConfigFilePath}'.");
        }

        var projectConfigFilePath = fileSystem.Path.Combine(projectDirectory, "FodyWeavers.xml");
        if (fileSystem.File.Exists(projectConfigFilePath))
        {
            files.Add(projectConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{projectConfigFilePath}'.");
        }

        if (files.Count == 0)
        {
            var pathsSearched = string.Join("', '", fodyDirConfigFilePath, solutionConfigFilePath, projectConfigFilePath);
            logger.LogDebug($"Could not find path to weavers file. Searched '{pathsSearched}'.");
        }

        return files;
    }

}