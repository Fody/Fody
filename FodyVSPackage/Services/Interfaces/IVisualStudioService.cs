namespace FodyVSPackage.Services
{
    using System;
    using EnvDTE;

    public interface IVisualStudioService
    {
        /// <summary>
        /// Gets the current solution.
        /// </summary>
        /// <returns>The <see cref="Solution"/> or <c>null</c> when there is currently no solution.</returns>
        Solution GetCurrentSolution();

        /// <summary>
        /// Gets all the projects of the current solution.
        /// </summary>
        /// <returns>All the projects of the current solution. If there is no current solution, this method will return an empty array.</returns>
        Project[] GetAllProjects();

        /// <summary>
        /// Gets the specified project by its name.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The <see cref="Project"/> or <c>null</c> if the project could not be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="projectName"/> is <c>null</c> or whitespace.</exception>
        Project GetProjectByName(string projectName);

        /// <summary>
        /// Gets the current project.
        /// </summary>
        /// <returns>The <see cref="Project"/> or <c>null</c> when there is no current project.</returns>
        Project GetCurrentProject();

        /// <summary>
        /// Gets the selected items in the solution explorer. All item names will be relative to the root.
        /// </summary>
        /// <returns>Array of the names of the selected items as relative paths to the root.</returns>
        /// <remarks>
        /// This method will determine the project based on the first selected item it finds. If files from multiple projects
        /// are selected, only the first found project files will be returned.
        /// </remarks>
        string[] GetSelectedItems();
    }
}