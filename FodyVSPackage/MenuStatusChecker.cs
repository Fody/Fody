using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

public class MenuStatusChecker
{
    CurrentProjectFinder currentProjectFinder;
    ExceptionDialog exceptionDialog;
    ContainsFodyChecker containsFodyChecker;

    public MenuStatusChecker(CurrentProjectFinder currentProjectFinder, ExceptionDialog exceptionDialog, ContainsFodyChecker containsFodyChecker)
    {
        this.currentProjectFinder = currentProjectFinder;
        this.exceptionDialog = exceptionDialog;
        this.containsFodyChecker = containsFodyChecker;
    }

    public void DisableCommandStatusCheck(OleMenuCommand disableCommand)
    {
        try
        {
            disableCommand.Enabled = false;
            foreach (var project in currentProjectFinder.GetCurrentProjects())
            {
                var xmlForProject = LoadXmlForProject(project);
                if (xmlForProject != null)
                {
                    if (containsFodyChecker.Check(xmlForProject))
                    {
                        disableCommand.Enabled = true;
                        return;
                    }
                }
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
    public void ConfigureCommandStatusCheck(OleMenuCommand configureCommand)
    {
        try
        {
            configureCommand.Enabled = false;
            foreach (var project in currentProjectFinder.GetCurrentProjects())
            {
                var xmlForProject = LoadXmlForProject(project);
                if (xmlForProject != null)
                {
                    if (!containsFodyChecker.Check(xmlForProject))
                    {
                        configureCommand.Enabled = true;
                        return;
                    }
                }
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

    static XDocument LoadXmlForProject(Project project)
    {
        string fullName;
        try
        {
            fullName = project.FullName;
        }
        catch (NotImplementedException)
        {
            //HACK: can happen during an upgrade from VS 2008
            return null;
        }
        //cant add to deployment projects
        if (fullName.EndsWith(".vdproj"))
        {
            return null;
        }
        //HACK: for when VS incorrectly calls configure when no project is avaliable
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }
        //HACK: for web projects
        if (!File.Exists(fullName))
        {
            return null;
        }
        try
        {
            //validate is xml
            return XDocument.Load(fullName);
        }
        catch (Exception)
        {
            //this means it is not xml and we cant do anything with it
            return null;
        }
    }
}