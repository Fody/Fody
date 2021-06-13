using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Processor
{
    public string AssemblyFilePath = null!;
    public string IntermediateDirectory = null!;
    public string? KeyFilePath;
    public bool SignAssembly;
    public bool DelaySign;
    public string ProjectDirectory = null!;
    public string ProjectFilePath = null!;
    public string? DocumentationFilePath;
    public string References = null!;
    public string SolutionDirectory = null!;
    public List<WeaverEntry> Weavers = null!;
    public string? WeaverConfiguration;

    public List<string> ReferenceCopyLocalPaths = null!;
    public List<string> DefineConstants = null!;

    public List<WeaverConfigFile> ConfigFiles = null!;
    public Dictionary<string, WeaverConfigEntry> ConfigEntries = null!;
    public bool GenerateXsd;
    IInnerWeaver? innerWeaver;

    static Dictionary<string, IsolatedAssemblyLoadContext> solutionAssemblyLoadContexts =
        new Dictionary<string, IsolatedAssemblyLoadContext>(StringComparer.OrdinalIgnoreCase);

    public ILogger Logger = null!;
    static readonly object mutex = new object();

    static Processor()
    {
        DomainAssemblyResolver.Connect();
    }

    public virtual bool Execute()
    {
        var assembly = typeof(Processor).Assembly;

        Logger.LogInfo($"Fody (version {assembly.GetName().Version} @ {assembly.CodeBase}) Executing");

        var stopwatch = Stopwatch.StartNew();

        try
        {
            Inner();
            return !Logger.ErrorOccurred;
        }
        catch (Exception exception)
        {
            Logger.LogException(exception);
            return false;
        }
        finally
        {
            stopwatch.Stop();
            Logger.LogInfo($"Finished Fody {stopwatch.ElapsedMilliseconds}ms.");
        }
    }

    void Inner()
    {
        ValidateSolutionPath();
        ValidateProjectPath();
        ValidateAssemblyPath();

        ConfigFiles = ConfigFileFinder.FindWeaverConfigFiles(WeaverConfiguration, SolutionDirectory, ProjectDirectory, Logger).ToList();

        if (!ConfigFiles.Any())
        {
            ConfigFiles = new List<WeaverConfigFile>
            {
                ConfigFileFinder.GenerateDefault(ProjectDirectory, Weavers, GenerateXsd)
            };
            Logger.LogWarning($"Could not find a FodyWeavers.xml file at the project level ({ProjectDirectory}). A default file has been created. Please review the file and add it to your project.");
        }

        ConfigEntries = ConfigFileFinder.ParseWeaverConfigEntries(ConfigFiles);

        var extraEntries = ConfigEntries.Values
            .Where(entry => !entry.ConfigFile.AllowExtraEntries && !Weavers.Any(weaver => string.Equals(weaver.ElementName, entry.ElementName)))
            .ToArray();

        const string missingWeaversHelp = "Add the desired weavers via their nuget package.";

        if (extraEntries.Any())
        {
            throw new WeavingException($"No weavers found for the configuration entries {string.Join(", ", extraEntries.Select(e => e.ElementName))}. " + missingWeaversHelp);
        }

        if (Weavers.Count == 0)
        {
            throw new WeavingException("No weavers found. " + missingWeaversHelp);
        }

        foreach (var weaver in Weavers)
        {
            if (ConfigEntries.TryGetValue(weaver.ElementName, out var config))
            {
                weaver.Element = config.Content;
                weaver.ConfigurationSource = config.ConfigFile.FilePath ?? "MSBuild property";
                weaver.ExecutionOrder = config.ExecutionOrder;
            }
            else
            {
                Logger.LogWarning($"No configuration entry found for the installed weaver {weaver.ElementName}. This weaver will be skipped. You may want to add this weaver to your FodyWeavers.xml");
            }
        }

        ConfigFileFinder.EnsureSchemaIsUpToDate(ProjectDirectory, Weavers, GenerateXsd);

        Weavers = Weavers
            .Where(weaver => weaver.Element != null)
            .OrderBy(weaver => weaver.ExecutionOrder)
            .ToList();

        lock (mutex)
        {
            ExecuteInOwnAssemblyLoadContext();
        }
    }

    void ExecuteInOwnAssemblyLoadContext()
    {
        var loadContext = GetLoadContext();

        using (innerWeaver = loadContext.CreateInstanceFromAndUnwrap())
        {
            innerWeaver.AssemblyFilePath = AssemblyFilePath;
            innerWeaver.References = References;
            innerWeaver.KeyFilePath = KeyFilePath;
            innerWeaver.ReferenceCopyLocalPaths = ReferenceCopyLocalPaths;
            innerWeaver.SignAssembly = SignAssembly;
            innerWeaver.DelaySign = DelaySign;
            innerWeaver.Logger = Logger;
            innerWeaver.SolutionDirectoryPath = SolutionDirectory;
            innerWeaver.Weavers = Weavers;
            innerWeaver.IntermediateDirectoryPath = IntermediateDirectory;
            innerWeaver.DefineConstants = DefineConstants;
            innerWeaver.ProjectDirectoryPath = ProjectDirectory;
            innerWeaver.ProjectFilePath = ProjectFilePath;
            innerWeaver.DocumentationFilePath = DocumentationFilePath;

            innerWeaver.Execute();

            ReferenceCopyLocalPaths = innerWeaver.ReferenceCopyLocalPaths;
        }
        innerWeaver = null;
    }

    IsolatedAssemblyLoadContext GetLoadContext()
    {
        if (solutionAssemblyLoadContexts.TryGetValue(SolutionDirectory, out var loadContext))
        {
            if (!WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
            {
                return loadContext;
            }

            Logger.LogDebug("A Weaver HasChanged so loading a new AssemblyLoadContext");
            loadContext.Unload();
        }

        return solutionAssemblyLoadContexts[SolutionDirectory] = CreateAssemblyLoadContext();
    }

    IsolatedAssemblyLoadContext CreateAssemblyLoadContext()
    {
        Logger.LogDebug("Creating a new AssemblyLoadContext");
        return new IsolatedAssemblyLoadContext();
    }

    public void Cancel()
    {
        innerWeaver?.Cancel();
    }
}
