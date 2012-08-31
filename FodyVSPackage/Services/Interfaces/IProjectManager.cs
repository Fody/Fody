using System.Collections.Generic;
using FodyVSPackage.Models;

namespace FodyVSPackage.Services
{
    public interface IProjectManager
    {
        /// <summary>
        /// Loads the projects in the current solution.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISolutionItem> LoadProjects();

        bool SaveProjects(IEnumerable<IProject> projects);
        bool IsAddinEnabledForProject(IProject project, IFodyAddin fodyAddin);
    }
}