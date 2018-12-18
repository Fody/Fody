using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Fody;

public partial class Processor
{
    public string AssemblyFilePath;
    public string IntermediateDirectory;
    public string KeyFilePath;
    public bool SignAssembly;
    public string ProjectDirectory;
    public string DocumentationFilePath;
    public string References;
    public string SolutionDirectory;
    public IList<WeaverEntry> Weavers;
    public DebugSymbolsType DebugSymbols;
    public List<string> ReferenceCopyLocalPaths;
    public List<string> DefineConstants;

    public IList<WeaverConfigFile> ConfigFiles;
    public IDictionary<string, WeaverConfigEntry> ConfigEntries;
    public bool GenerateXsd;
    IInnerWeaver innerWeaver;

    static Dictionary<string, IsolatedAssemblyLoadContext> solutionAssemblyLoadContexts =
        new Dictionary<string, IsolatedAssemblyLoadContext>(StringComparer.OrdinalIgnoreCase);

    public BuildLogger Logger;
    static readonly object mutex = new object();

    public ContainsTypeChecker ContainsTypeChecker = new ContainsTypeChecker();

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
            Logger.LogInfo($"  Finished Fody {stopwatch.ElapsedMilliseconds}ms.");
        }
    }

    void Inner()
    {
        ValidateSolutionPath();
        ValidateProjectPath();
        ValidateAssemblyPath();

        ConfigFiles = ConfigFile.FindWeaverConfigFiles(SolutionDirectory, ProjectDirectory, Logger).ToArray();

        if (!ConfigFiles.Any())
        {
            ConfigFiles = new[] { ConfigFile.GenerateDefault(ProjectDirectory, Weavers, GenerateXsd) };
            Logger.LogWarning($"Could not find a FodyWeavers.xml file at the project level ({ProjectDirectory}). A default file has been created. Please review the file and add it to your project.");
        }

        ConfigEntries = ConfigFile.ParseWeaverConfigEntries(ConfigFiles, Logger);

        var extraEntries = ConfigEntries.Values
            .Where(entry => !entry.ConfigFile.IsGlobal && !Weavers.Any(weaver => string.Equals(weaver.ElementName, entry.ElementName)))
            .ToArray();

        const string missingWeaversHelp = "Add the desired weavers via their nuget package; see https://github.com/Fody/Fody/wiki on how to migrate InSolution, custom or legacy weavers.";

        if (extraEntries.Any())
        {
            throw new WeavingException($"No weavers found for the configuration entries {string.Join(", ", extraEntries.Select(e => e.ElementName))}. " + missingWeaversHelp);
        }

        if (Weavers.Count == 0)
        {
            throw new WeavingException(@"No weavers found." + missingWeaversHelp);
        }

        foreach (var weaver in Weavers)
        {
            if (ConfigEntries.TryGetValue(weaver.ElementName, out var config))
            {
                weaver.Element = config.Content;
                weaver.ExecutionOrder = config.ExecutionOrder;
            }
            else
            {
                throw new WeavingException($"No configuration entry found for the installed weaver {weaver.ElementName}. You need to add this weaver to your FodyWeavers.xml");
            }
        }

        if (TargetAssemblyHasAlreadyBeenProcessed())
        {
            if (WeaversConfigHistory.HasChanged(ConfigFiles) || WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
            {
                Logger.LogError("A re-build is required because a weaver has changed.");

                return;
            }
        }

        ConfigFile.EnsureSchemaIsUpToDate(ProjectDirectory, Weavers, GenerateXsd);

        lock (mutex)
        {
            ExecuteInOwnAssemblyLoadContext();
        }

        WeaversConfigHistory.RegisterSnapshot(ConfigFiles);
    }

    void ExecuteInOwnAssemblyLoadContext()
    {
        if (solutionAssemblyLoadContexts.TryGetValue(SolutionDirectory, out var loadContext))
        {
            if (WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
            {
                Logger.LogDebug("A Weaver HasChanged so loading a new AssemblyLoadContext");
                loadContext.Unload();
                loadContext = solutionAssemblyLoadContexts[SolutionDirectory] = CreateAssemblyLoadContext();
            }
        }
        else
        {
            loadContext = solutionAssemblyLoadContexts[SolutionDirectory] = CreateAssemblyLoadContext();
        }

        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        using (innerWeaver = (IInnerWeaver)loadContext.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver"))
        {
            innerWeaver.AssemblyFilePath = AssemblyFilePath;
            innerWeaver.References = References;
            innerWeaver.KeyFilePath = KeyFilePath;
            innerWeaver.ReferenceCopyLocalPaths = ReferenceCopyLocalPaths;
            innerWeaver.SignAssembly = SignAssembly;
            innerWeaver.Logger = Logger;
            innerWeaver.SolutionDirectoryPath = SolutionDirectory;
            innerWeaver.Weavers = Weavers.OrderBy(weaver => weaver.ExecutionOrder).ToArray();
            innerWeaver.IntermediateDirectoryPath = IntermediateDirectory;
            innerWeaver.DefineConstants = DefineConstants;
            innerWeaver.ProjectDirectoryPath = ProjectDirectory;
            innerWeaver.DocumentationFilePath = DocumentationFilePath;
            innerWeaver.DebugSymbols = DebugSymbols;

            innerWeaver.Execute();

            ReferenceCopyLocalPaths = innerWeaver.ReferenceCopyLocalPaths;
        }
        innerWeaver = null;
    }

    IsolatedAssemblyLoadContext CreateAssemblyLoadContext()
    {
        Logger.LogDebug("Creating a new AssemblyLoadContext");
        return new IsolatedAssemblyLoadContext($"Fody Domain for '{SolutionDirectory}'", AssemblyLocation.CurrentDirectory);
    }

    public void Cancel()
    {
        innerWeaver?.Cancel();
    }
}