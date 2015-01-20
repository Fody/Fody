using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

public class Configuration
{
    private readonly ILogger _logger;
    private readonly string _solutionDirectory;
    private readonly string _projectDirectory;

    public Configuration(ILogger logger, string solutionDirectory, string projectDirectory, List<string> defineConstants)
    {
        _logger = logger;
        _solutionDirectory = solutionDirectory;
        _projectDirectory = projectDirectory;
        DefineConstants = defineConstants;

        ConfigFiles = new List<string>();

        LoadConfiguration();
    }

    public List<string> ConfigFiles { get; private set; }

    public List<string> DefineConstants { get; private set; }

    public bool VerifyAssembly { get; private set; }

    public virtual void LoadConfiguration()
    {
        var fodyDirConfigFilePath = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyWeavers.xml");
        if (File.Exists(fodyDirConfigFilePath))
        {
            ConfigFiles.Add(fodyDirConfigFilePath);
            _logger.LogDebug(string.Format("Found path to weavers file '{0}'.", fodyDirConfigFilePath));
        }

        var solutionConfigFilePath = Path.Combine(_solutionDirectory, "FodyWeavers.xml");
        if (File.Exists(solutionConfigFilePath))
        {
            ConfigFiles.Add(solutionConfigFilePath);
            _logger.LogDebug(string.Format("Found path to weavers file '{0}'.", solutionConfigFilePath));
        }

        var projectConfigFilePath = Path.Combine(_projectDirectory, "FodyWeavers.xml");
        if (File.Exists(projectConfigFilePath))
        {
            ConfigFiles.Add(projectConfigFilePath);
            _logger.LogDebug(string.Format("Found path to weavers file '{0}'.", projectConfigFilePath));
        }

        if (ConfigFiles.Count == 0)
        {
            var pathsSearched = string.Join("', '", fodyDirConfigFilePath, solutionConfigFilePath, projectConfigFilePath);
            _logger.LogDebug(string.Format("Could not find path to weavers file. Searched '{0}'.", pathsSearched));
        }

        try
        {
            foreach (var configFile in ConfigFiles)
            {
                var configXml = XDocument.Load(configFile);
                var element = configXml.Root;

                element.ReadBool("VerifyAssembly", x => VerifyAssembly = x);
            }
        }
        catch (Exception)
        {
            _logger.LogInfo("Failed to read config, using default configuration");
        }
    }
}