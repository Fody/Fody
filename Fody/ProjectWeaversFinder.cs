using System.Collections.Generic;
using System.IO;
using Fody;

public class ProjectWeaversFinder
{
    public const string FodyWeaversXml = "FodyWeavers.xml";
    public ProjectPathFinder ProjectPathFinder;
    public WeavingTask WeavingTask;
    public BuildLogger Logger;
    public List<string> ConfigFiles = new List<string>();

    public string SolutionConfigFilePath;


    public void Execute()
    {
        var fodyDirConfigFilePath = Path.Combine(AssemblyLocation.CurrentDirectory(), FodyWeaversXml);
        if (File.Exists(fodyDirConfigFilePath))
        {
            ConfigFiles.Add(fodyDirConfigFilePath);
            Logger.LogInfo(string.Format("Found path to weavers file '{0}'.", fodyDirConfigFilePath));
        }

        var solutionDirectory = WeavingTask.SolutionDir;
        var solutionConfigFilePath = Path.Combine(solutionDirectory, FodyWeaversXml);
        if (File.Exists(solutionConfigFilePath))
        {
            ConfigFiles.Add(solutionConfigFilePath);
            Logger.LogInfo(string.Format("Found path to weavers file '{0}'.", solutionConfigFilePath));
        }


        var projectDirectory = Path.GetDirectoryName(ProjectPathFinder.ProjectFilePath);
        var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversXml);
        if (File.Exists(projectConfigFilePath))
        {
            ConfigFiles.Add(projectConfigFilePath);
            Logger.LogInfo(string.Format("Found path to weavers file '{0}'.", projectConfigFilePath));
        }


        if (ConfigFiles.Count > 0)
        {
            var pathsSearched = string.Join(", ", fodyDirConfigFilePath, solutionConfigFilePath, projectConfigFilePath);
            Logger.LogInfo(string.Format("Could not find path to weavers file. Searched '{0}'.", pathsSearched));
        }
    }

}