namespace FodyVSPackage.Services
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using EnvDTE;

    public class VisualStudioService : IVisualStudioService
    {
        private readonly DTE _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioService"/> class.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dte"/> is <c>null</c>.</exception>
        public VisualStudioService(DTE dte)
        {
            Argument.IsNotNull("dte", dte);

            _dte = dte;
        }

        /// <summary>
        /// Gets the current solution.
        /// </summary>
        /// <returns>The <see cref="Solution"/> or <c>null</c> when there is currently no solution.</returns>
        public Solution GetCurrentSolution()
        {
            return _dte.Solution;
        }

        /// <summary>
        /// Gets the current project.
        /// </summary>
        /// <returns>The <see cref="Project"/> or <c>null</c> when there is no current project.</returns>
        public Project GetCurrentProject()
        {
            return _dte.GetActiveProject();
        }

        /// <summary>
        /// Gets all the projects of the current solution.
        /// </summary>
        /// <returns>All the projects of the current solution. If there is no current solution, this method will return an empty array.</returns>
        public Project[] GetAllProjects()
        {
            var projects = new List<Project>();

            var solution = GetCurrentSolution();
            if (solution != null)
            {
                projects.AddRange(solution.GetAllProjects());
            }

            return projects.ToArray();
        }

        /// <summary>
        /// Gets the specified project by its name.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The <see cref="Project"/> or <c>null</c> if the project could not be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="projectName"/> is <c>null</c> or whitespace.</exception>
        public Project GetProjectByName(string projectName)
        {
            Argument.IsNotNullOrWhitespace("projectName", projectName);

            var solution = GetCurrentSolution();
            if (solution == null)
            {
                return null;
            }

            return solution.GetProjectFromSolution(projectName);
        }

        /// <summary>
        /// Gets the selected items in the solution explorer. All item names will be relative to the root.
        /// </summary>
        /// <returns>Array of the names of the selected items as relative paths to the root.</returns>
        /// <remarks>
        /// This method will only return files from the current project. If files from multiple projects
        /// are selected, only the files that belong to the current project will be returned.
        /// </remarks>
        public string[] GetSelectedItems()
        {
            var selectedItems = new List<string>();

            Project selectedProject = GetCurrentProject();
            if (selectedProject == null)
            {
                return selectedItems.ToArray();
            }

            foreach (SelectedItem selectedItem in _dte.SelectedItems)
            {
                ProjectItem selectedProjectItem = selectedItem.ProjectItem;
                if (selectedProjectItem != null)
                {
                    if (string.Compare(selectedProject.Name, selectedProjectItem.ContainingProject.Name) == 0)
                    {
                        selectedItems.Add(selectedItem.ProjectItem.GetNameRelativeToRoot());
                    }                    
                }
            }

            return selectedItems.ToArray();
        }
    }
}