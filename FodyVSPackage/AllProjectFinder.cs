using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

public class AllProjectFinder
{
    public IEnumerable<Project> GetAllProjects()
    {
        var projectList = new List<Project>();
        var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
        foreach (Project project in dte.Solution.Projects)
        {
            if (ProjectKind.IsSupportedProjectKind(project.Kind))
            {
                projectList.Add(project);
            }
            FindProjectInternal(project.ProjectItems, projectList);
        }
        return projectList;
    }

    static void FindProjectInternal(ProjectItems items, List<Project> projectList)
    {
        if (items == null)
        {
            return;
        }

        foreach (ProjectItem item in items)
        {
            Project project;
            if (item.SubProject != null)
            {
                project = item.SubProject;
            }
            else
            {
                project = item.Object as Project;
            }
            if (project != null)
            {
                if (ProjectKind.IsSupportedProjectKind(project.Kind))
                {
                    projectList.Add(project);
                }
                FindProjectInternal(project.ProjectItems, projectList);
            }
        }
    }

}