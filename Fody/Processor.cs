using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;

public partial class Processor
{
    public string AssemblyFilePath;
    public string IntermediateDirectoryPath;
    public string KeyFilePath;
    public string MessageImportance = "Low";
    public string ProjectDirectory;
    public string References;
    public string SolutionDirectoryPath;
    public IBuildEngine BuildEngine;

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

    public bool Execute()
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("Fody (version {0}) Executing", GetType().Assembly.GetName().Version), "", "Fody", Microsoft.Build.Framework.MessageImportance.High));

        var stopwatch = Stopwatch.StartNew();

        Logger = new BuildLogger(MessageImportance)
                     {
                         BuildEngine = BuildEngine,
                     };

        try
        {
            Inner();
            return !Logger.ErrorOccurred;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception.ToFriendlyString());
            return false;
        }
        finally
        {
            stopwatch.Stop();
            Logger.Flush();
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("\tFinished Fody {0}ms.", stopwatch.ElapsedMilliseconds), "", "Fody", Microsoft.Build.Framework.MessageImportance.High));
        }
    }

    void Inner()
    {


        ValidateProjectPath();

        ValidatorAssemblyPath();


        

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

        if (Weavers.Count == 0)
        {
            Logger.LogWarning(string.Format("Could not find any weavers. Either add a project named 'Weavers' with a type named 'ModuleWeaver' or add some items to '{0}'.", "FodyWeavers.xml"));
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
    }


    void ExecuteInOwnAppDomain()
    {
        AppDomain appdomain;
        if (solutionDomains.TryGetValue(SolutionDirectoryPath, out appdomain))
        {
            if (WeaversHistory.HasChanged(Weavers.Select(x => x.AssemblyPath)))
            {
                Logger.LogInfo("A Weaver HasChanged so loading a new AppDomian");
                AppDomain.Unload(appdomain);
                appdomain = solutionDomains[SolutionDirectoryPath] = CreateDomain();
            }
        }
        else
        {
            appdomain = solutionDomains[SolutionDirectoryPath] = CreateDomain();
        }


        var innerWeaver = (IInnerWeaver) appdomain.CreateInstanceAndUnwrap("FodyIsolated", "InnerWeaver");
        innerWeaver.AssemblyFilePath = AssemblyFilePath;
        innerWeaver.References = References;
        innerWeaver.KeyFilePath = KeyFilePath;
        innerWeaver.Logger = Logger;
        innerWeaver.SolutionDirectoryPath = SolutionDirectoryPath;
        innerWeaver.Weavers = Weavers;
        innerWeaver.IntermediateDirectoryPath = IntermediateDirectoryPath;
        innerWeaver.Execute();
    }

    AppDomain CreateDomain()
    {
        Logger.LogInfo("Creating a new AppDomian");
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = AssemblyLocation.CurrentDirectory(),
        };
        return AppDomain.CreateDomain(string.Format("Fody Domain for '{0}'", SolutionDirectoryPath), null, appDomainSetup);
    }
}
