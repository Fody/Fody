using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

public class CurrentProjectFinder
{


    public  List<Project> GetCurrentProjects()
    {
        var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
        if (dte.Solution == null)
        {
            return new List<Project>();
        }
        if (string.IsNullOrEmpty(dte.Solution.FullName))
        {
            return new List<Project>();
        }
        try
        {
            var solutionProjects = (object[])dte.ActiveSolutionProjects;
            return solutionProjects.Cast<Project>().ToList();
        }
        catch (COMException)
        {
            return new List<Project>(); 
        }
    }

}