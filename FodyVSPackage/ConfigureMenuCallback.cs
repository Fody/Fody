using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

public class ConfigureMenuCallback
{
    ContentsFinder contentsFinder;
    CurrentProjectFinder currentProjectFinder;
    ExceptionDialog exceptionDialog;

    public ConfigureMenuCallback(CurrentProjectFinder currentProjectFinder, ContentsFinder contentsFinder, ExceptionDialog exceptionDialog)
    {
        this.currentProjectFinder = currentProjectFinder;
        this.exceptionDialog = exceptionDialog;
        this.contentsFinder = contentsFinder;
    }


    public void ConfigureCallback()
    {
        try
        {
            var currentProjects = currentProjectFinder.GetCurrentProjects();
            if (currentProjects
                .Any(UnsaveProjectChecker.HasUnsavedPendingChanges))
            {
                return;
            }
            foreach (var project in currentProjects)
            {
                Configure(project);
            }
        }
        catch (COMException exception)
        {
            exceptionDialog.HandleException(exception);
        }
        catch (Exception exception)
        {
            exceptionDialog.HandleException(exception);
        }
    }

    void Configure(Project project)
    {
        var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        var solutionDirectory = Path.GetDirectoryName(dte.Solution.FullName);
        var toolsDirectory = CreateToolsDirectory(solutionDirectory);
        ExportBuildFile(toolsDirectory);
        ExportFodyWeaversXml(project.FullName);
        //@"$(SolutionDir)\Tools\Fody\"
        var relativePath = PathEx.MakeRelativePath(project.FullName, toolsDirectory);
        InjectIntoProject(project.FullName, Path.Combine("$(ProjectPath)", relativePath));
    }

    string CreateToolsDirectory(string solutionDirectory)
    {
        var fodyDirectory = FodyDirectoryFinder.TreeWalkForToolsFodyDir(solutionDirectory);
        if (fodyDirectory != null)
        {
            return fodyDirectory;
        }
        var packagesPath = NugetConfigReader.GetPackagesPathFromConfig(solutionDirectory);

        if (packagesPath != null)
        {
            fodyDirectory = Path.Combine(Directory.GetParent(packagesPath).FullName, @"Tools\Fody");
        }
        else
        {
            fodyDirectory = Path.Combine(solutionDirectory, @"Tools\Fody");
        }
        if (!Directory.Exists(fodyDirectory))
        {
            Directory.CreateDirectory(fodyDirectory);
        }
        return fodyDirectory;
    }

    void ExportFodyWeaversXml(string projectFilePath)
    {
        var projectDirectory = Path.GetDirectoryName(projectFilePath);
        var tasksFile = Path.Combine(projectDirectory, "FodyWeavers.xml");
        if (File.Exists(tasksFile))
        {
            return;
        }
        var path = Path.Combine(contentsFinder.ContentFilesPath, "FodyWeavers.xml");
        File.Copy(path, tasksFile);
    }

    void InjectIntoProject(string projectFilePath, string fodyToolsDirectory)
    {
        var projectInjector = new ProjectInjector
                                  {
                                      ProjectFile = projectFilePath,
                                      FodyToolsDirectory = fodyToolsDirectory 
                                  };
        projectInjector.Execute();
    }

    void ExportBuildFile(string toolsDirectory)
    {
        foreach (var file in Directory.GetFiles(Path.Combine(contentsFinder.ContentFilesPath, "Fody")))
        {
            var destFileName = Path.Combine(toolsDirectory, Path.GetFileName(file));
            if (!File.Exists(destFileName))
            {
                File.Copy(file, destFileName);
            }
        }
    }

}