using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fody.Verification;
using Microsoft.Build.Framework;
using MSMessageEnum = Microsoft.Build.Framework.MessageImportance;

public partial class Processor
{
    public string AssemblyFilePath;
    public string IntermediateDirectoryPath;
    public string KeyFilePath;
    public bool SignAssembly;
    public bool VerifyAssembly;
    public string ProjectDirectory;
    public string References;
    public string SolutionDirectoryPath;
    public IBuildEngine BuildEngine;
    public List<string> ReferenceCopyLocalPaths;
    public List<string> DefineConstants;

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
        Logger = new BuildLogger
        {
            BuildEngine = BuildEngine,
        };

        Logger.LogInfo(string.Format("Fody (version {0}) Executing", typeof(Processor).Assembly.GetName().Version));

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

        FindProjectWeavers();

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

        Configure();

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
                SolutionDirectoryPath = SolutionDirectoryPath
            };
        addinFinder.FindAddinDirectories();

        FindWeaverProjectFile();

        ConfigureWhenWeaversFound();

        ConfigureWhenNoWeaversFound();

        Logger.LogDebug(string.Format("Finished finding weavers {0}ms", stopwatch.ElapsedMilliseconds));
    }

    void Configure()
    {
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
            Logger.LogInfo("Failed to read config, using default configuration");
        }
    }

    void ExecuteInOwnAppDomain()
    {
        AppDomain appDomain;
        if (solutionDomains.TryGetValue(SolutionDirectoryPath, out appDomain))
        {
            if (WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
            {
                Logger.LogDebug("A Weaver HasChanged so loading a new AppDomain");
                AppDomain.Unload(appDomain);
                appDomain = solutionDomains[SolutionDirectoryPath] = CreateDomain();
            }
        }
        else
        {
            appDomain = solutionDomains[SolutionDirectoryPath] = CreateDomain();
        }

        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        using (var innerWeaver = (IInnerWeaver)appDomain.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver"))
        {
            innerWeaver.AssemblyFilePath = AssemblyFilePath;
            innerWeaver.References = References;
            innerWeaver.KeyFilePath = KeyFilePath;
            innerWeaver.ReferenceCopyLocalPaths = ReferenceCopyLocalPaths;
            innerWeaver.SignAssembly = SignAssembly;
            innerWeaver.VerifyAssembly = VerifyAssembly;
            innerWeaver.Logger = Logger;
            innerWeaver.SolutionDirectoryPath = SolutionDirectoryPath;
            innerWeaver.Weavers = Weavers;
            innerWeaver.IntermediateDirectoryPath = IntermediateDirectoryPath;
            innerWeaver.DefineConstants = DefineConstants;
            innerWeaver.ProjectDirectoryPath = ProjectDirectory;

            innerWeaver.Execute();
        }

#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif

        if (VerifyAssembly)
        {
            var verifier = new PeVerifier(Logger, References);
            verifier.Verify(AssemblyFilePath);
        }
    }

    AppDomain CreateDomain()
    {
        Logger.LogDebug("Creating a new AppDomain");
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = AssemblyLocation.CurrentDirectory,
        };
        return AppDomain.CreateDomain(string.Format("Fody Domain for '{0}'", SolutionDirectoryPath), null, appDomainSetup);
    }
}