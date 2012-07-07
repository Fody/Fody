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
        var toolsDirectory = CreateToolsDirectory();
        ExportBuildFile(toolsDirectory);
        ExportFodyWeaversXml(project.FullName);
        InjectIntoProject(project.FullName);
    }

    void ExportFodyWeaversXml(string projectFilePath)
    {
        var projectDirectory = Path.GetDirectoryName(projectFilePath);
        var tasksFile = Path.Combine(projectDirectory, ConfigFile.FodyWeaversXml);
        if (File.Exists(tasksFile))
        {
            return;
        }
        var path = Path.Combine(contentsFinder.ContentFilesPath, ConfigFile.FodyWeaversXml);
        File.Copy(path, tasksFile);
    }

    void InjectIntoProject(string projectFilePath)
    {
        var projectInjector = new ProjectInjector
                                  {
                                      ProjectFile = projectFilePath
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

    string CreateToolsDirectory()
    {
        var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        var toolsDirectory = Path.Combine(Path.GetDirectoryName(dte.Solution.FullName), @"Tools\Fody");
        if (!Directory.Exists(toolsDirectory))
        {
            Directory.CreateDirectory(toolsDirectory);
        }
        return toolsDirectory;
    }
}