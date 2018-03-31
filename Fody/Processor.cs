using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
    public string NuGetPackageRoot;
    public string MSBuildDirectory;
    public bool DebugSymbols;
    public List<string> ReferenceCopyLocalPaths;
    public List<string> PackageDefinitions;
    public List<string> DefineConstants;
    public List<string> ConfigFiles;
    InnerWeaver innerWeaver;

    AddinFinder addinFinder;
    static Dictionary<string, IsolatedAssemblyLoadContext> solutionAssemblyLoadContexts =
        new Dictionary<string, IsolatedAssemblyLoadContext>(StringComparer.OrdinalIgnoreCase);

    public BuildLogger Logger;
    static object locker;

    public ContainsTypeChecker ContainsTypeChecker = new ContainsTypeChecker();

    static Processor()
    {
        locker = new object();
        DomainAssemblyResolver.Connect();
    }

    public virtual bool Execute()
    {
        Logger.LogInfo($"Fody (version {typeof (Processor).Assembly.GetName().Version}) Executing");

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
            Logger.LogInfo($"  Finished Fody {stopwatch.ElapsedMilliseconds}ms.");
        }
    }

    void Inner()
    {
        ValidateProjectPath();

        ValidateAssemblyPath();

        ConfigFiles = ConfigFileFinder.FindWeaverConfigs(SolutionDirectory, ProjectDirectory, Logger);

        if (!ShouldStartSinceFileChanged())
        {
            if (!CheckForWeaversXmlChanged())
            {

                FindWeavers();

                if (WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
                {
                    Logger.LogError("A re-build is required because a weaver has changed.");
                }
            }
            return;
        }

        ValidateSolutionPath();

        FindWeavers();

        if (Weavers.Count == 0)
        {
            Logger.LogWarning(@"No configured weavers. It is possible no weavers have been installed or a weaver has been installed into a project type that does not support install.ps1. It may be necessary to manually add that weaver to FodyWeavers.xm;. eg.
<Weavers>
    <WeaverName/>
</Weavers>
see https://github.com/Fody/Fody/wiki/SampleUsage");
            return;
        }
        lock (locker)
        {
            ExecuteInOwnAssemblyLoadContext();
        }

        FlushWeaversXmlHistory();
    }

    void FindWeavers()
    {
        var stopwatch = Stopwatch.StartNew();
        Logger.LogDebug("Finding weavers");
        ReadProjectWeavers();
        addinFinder = new AddinFinder(Logger.LogDebug, SolutionDirectory, MSBuildDirectory, NuGetPackageRoot, PackageDefinitions);
        addinFinder.FindAddinDirectories();

        FindWeaverProjectFile();

        ConfigureWhenWeaversFound();

        ConfigureWhenNoWeaversFound();

        Logger.LogDebug($"Finished finding weavers {stopwatch.ElapsedMilliseconds}ms");
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
        using (innerWeaver = (InnerWeaver)loadContext.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver"))
        {
            innerWeaver.AssemblyFilePath = AssemblyFilePath;
            innerWeaver.References = References;
            innerWeaver.KeyFilePath = KeyFilePath;
            innerWeaver.ReferenceCopyLocalPaths = ReferenceCopyLocalPaths;
            innerWeaver.SignAssembly = SignAssembly;
            innerWeaver.Logger = Logger;
            innerWeaver.SolutionDirectoryPath = SolutionDirectory;
            innerWeaver.Weavers = Weavers;
            innerWeaver.IntermediateDirectoryPath = IntermediateDirectory;
            innerWeaver.DefineConstants = DefineConstants;
            innerWeaver.ProjectDirectoryPath = ProjectDirectory;
            innerWeaver.DocumentationFilePath = DocumentationFilePath;
            innerWeaver.DebugSymbols = DebugSymbols;

            innerWeaver.Execute();
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