using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MSMessageEnum = Microsoft.Build.Framework.MessageImportance;

public partial class Processor
{
    public string AssemblyFilePath;
    public string IntermediateDirectory;
    public string KeyFilePath;
    public bool SignAssembly;
    public string ProjectDirectory;
    public string References;
    public string SolutionDirectory;
    public string NuGetPackageRoot;
    public bool DebugSymbols;
    public List<string> ReferenceCopyLocalPaths;
    public List<string> PackageDefinitions;
    public List<string> DefineConstants;
    public List<string> ConfigFiles;
    IInnerWeaver innerWeaver;

    AddinFinder addinFinder;
    static Dictionary<string, AppDomain> solutionDomains = new Dictionary<string, AppDomain>(StringComparer.OrdinalIgnoreCase);

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
            ExecuteInOwnAppDomain();
        }

        FlushWeaversXmlHistory();
    }


    void FindWeavers()
    {
        var stopwatch = Stopwatch.StartNew();
        Logger.LogDebug("Finding weavers");
        ReadProjectWeavers();
        addinFinder = new AddinFinder
            {
                Logger = Logger,
                SolutionDirectoryPath = SolutionDirectory,
                NuGetPackageRoot = NuGetPackageRoot,
                PackageDefinitions = PackageDefinitions,
            };
        addinFinder.FindAddinDirectories();

        FindWeaverProjectFile();

        ConfigureWhenWeaversFound();

        ConfigureWhenNoWeaversFound();

        Logger.LogDebug($"Finished finding weavers {stopwatch.ElapsedMilliseconds}ms");
    }

    void ExecuteInOwnAppDomain()
    {
        AppDomain appDomain;
        if (solutionDomains.TryGetValue(SolutionDirectory, out appDomain))
        {
            if (WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
            {
                Logger.LogDebug("A Weaver HasChanged so loading a new AppDomain");
                AppDomain.Unload(appDomain);
                appDomain = solutionDomains[SolutionDirectory] = CreateDomain();
            }
        }
        else
        {
            appDomain = solutionDomains[SolutionDirectory] = CreateDomain();
        }

        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        using (innerWeaver = (IInnerWeaver)appDomain.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver"))
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
            innerWeaver.DebugSymbols = DebugSymbols;

            innerWeaver.Execute();
        }
        innerWeaver = null;
    }

    AppDomain CreateDomain()
    {
        Logger.LogDebug("Creating a new AppDomain");
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = AssemblyLocation.CurrentDirectory,
        };
        return AppDomain.CreateDomain($"Fody Domain for '{SolutionDirectory}'", null, appDomainSetup);
    }

    public void Cancel()
    {
        innerWeaver?.Cancel();
    }
}