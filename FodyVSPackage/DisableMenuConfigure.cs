using System;
using System.Linq;
using System.Runtime.InteropServices;

public class DisableMenuConfigure
{
    CurrentProjectFinder currentProjectFinder;
    MessageDisplayer messageDisplayer;
    ExceptionDialog exceptionDialog;

    public DisableMenuConfigure(CurrentProjectFinder currentProjectFinder, MessageDisplayer messageDisplayer, ExceptionDialog exceptionDialog)
    {
        this.exceptionDialog = exceptionDialog;
        this.messageDisplayer = messageDisplayer;
        this.currentProjectFinder = currentProjectFinder;
    }

    public void DisableCallback()
    {
        try
        {
            var projects = currentProjectFinder.GetCurrentProjects();
            if (projects.Any(UnsaveProjectChecker.HasUnsavedPendingChanges))
            {
                return;
            }
            foreach (var project in projects)
            {
                messageDisplayer.ShowInfo(string.Format("Fody: Removed from the project '{0}'. However no binary files will be removed in case they are being used by other projects.", project.Name));
                new ProjectRemover(project.FullName);   
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

}