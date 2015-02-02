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
    public List<string> ReferenceCopyLocalPaths;
    public List<string> DefineConstants;
    public List<string> ConfigFiles;

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

        Logger.LogInfo(string.Format("Fody (version {0}) Executing", typeof (Processor).Assembly.GetName().Version));

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
            Logger.LogInfo(string.Format("  Finished Fody {0}ms.", stopwatch.ElapsedMilliseconds));
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
                    Logger.LogWarning("A re-build is required to because a weaver changed");
                }
            }
            return;
        }

        ValidateSolutionPath();

        FindWeavers();

        if (Weavers.Count == 0)
        {
            Logger.LogError("You don't seem to have configured any weavers. Try adding a Fody nuget package to your project. Have a look here http://nuget.org/packages?q=fody for the list of available packages.");
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
                SolutionDirectoryPath = SolutionDirectory
            };
        addinFinder.FindAddinDirectories();

        FindWeaverProjectFile();

        ConfigureWhenWeaversFound();

        ConfigureWhenNoWeaversFound();

        Logger.LogDebug(string.Format("Finished finding weavers {0}ms", stopwatch.ElapsedMilliseconds));
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
        using (var innerWeaver = (IInnerWeaver)appDomain.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver"))
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

            innerWeaver.Execute();
        }
    }

    AppDomain CreateDomain()
    {
        Logger.LogDebug("Creating a new AppDomain");
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = AssemblyLocation.CurrentDirectory,
        };
        return AppDomain.CreateDomain(string.Format("Fody Domain for '{0}'", SolutionDirectory), null, appDomainSetup);
    }
}