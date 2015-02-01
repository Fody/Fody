using System.Collections.Generic;
using System.IO;

public  class ConfigFileFinder
{
    
    public static List<string> FindProjectWeavers(string solutionDirectoryPath, string projectDirectory, BuildLogger logger)
    {
        var files = new List<string>();
        var fodyDirConfigFilePath = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyWeavers.xml");
        if (File.Exists(fodyDirConfigFilePath))
        {
            files.Add(fodyDirConfigFilePath);
            logger.LogDebug(string.Format("Found path to weavers file '{0}'.", fodyDirConfigFilePath));
        }

        var solutionConfigFilePath = Path.Combine(solutionDirectoryPath, "FodyWeavers.xml");
        if (File.Exists(solutionConfigFilePath))
        {
            files.Add(solutionConfigFilePath);
            logger.LogDebug(string.Format("Found path to weavers file '{0}'.", solutionConfigFilePath));
        }

		var projectConfigFilePath = Path.Combine(projectDirectory, "FodyWeavers.xml");
        if (File.Exists(projectConfigFilePath))
        {
            files.Add(projectConfigFilePath);
            logger.LogDebug(string.Format("Found path to weavers file '{0}'.", projectConfigFilePath));
        }


        if (files.Count == 0)
        {
            var pathsSearched = string.Join("', '", fodyDirConfigFilePath, solutionConfigFilePath, projectConfigFilePath);
            logger.LogDebug(string.Format("Could not find path to weavers file. Searched '{0}'.", pathsSearched));
        }
        return files;
    }

}