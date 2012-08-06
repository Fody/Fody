using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

public class SolutionEvents : IVsSolutionEvents
{
    TaskFileProcessor taskFileProcessor;
    ExceptionDialog exceptionDialog;
    MSBuildKiller msBuildKiller;

    public SolutionEvents(TaskFileProcessor taskFileProcessor, ExceptionDialog exceptionDialog, MSBuildKiller msBuildKiller)
    {
        this.taskFileProcessor = taskFileProcessor;
        this.exceptionDialog = exceptionDialog;
        this.msBuildKiller = msBuildKiller;
    }

    public void RegisterSolutionEvents()
    {
        uint cookie;
        var vsSolution = (IVsSolution) ServiceProvider.GlobalProvider.GetService(typeof (IVsSolution));
        vsSolution.AdviseSolutionEvents(this, out cookie);
    }


    public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
    {
        try
        {
            var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
            if (!string.IsNullOrEmpty(dte.Solution.FullName))
            {
                var solutionDirectory = Path.GetDirectoryName(dte.Solution.FullName);
                taskFileProcessor.ProcessTaskFile(solutionDirectory);
            }
        }
        catch (Exception exception)
        {
            exceptionDialog.HandleException(exception);
        }
        return VSConstants.S_OK;
    }

    public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
    {
        return VSConstants.S_OK;
    }

    public int OnBeforeCloseSolution(object pUnkReserved)
    {
        return VSConstants.S_OK;
    }

    public int OnAfterCloseSolution(object pUnkReserved)
    {
        msBuildKiller.Kill();
        return VSConstants.S_OK;
    }

    public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
    {
        return VSConstants.S_OK;
    }

    public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
    {
        return VSConstants.S_OK;
    }

    public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
    {
        return VSConstants.S_OK;
    }

    public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
    {
        return VSConstants.S_OK;
    }

    public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
    {
        return VSConstants.S_OK;
    }

    public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
    {
        return VSConstants.S_OK;
    }

}