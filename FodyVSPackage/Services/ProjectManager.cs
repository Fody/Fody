using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Catel;
using Catel.Logging;
using FodyVSPackage.Models;

namespace FodyVSPackage.Services
{
    public class ProjectManager : IProjectManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IVisualStudioService _visualStudioService;
        private readonly IFodyAddinManager _fodyAddinManager;

        public ProjectManager(IVisualStudioService visualStudioService, IFodyAddinManager fodyAddinManager)
        {
            Argument.IsNotNull("visualStudioService", visualStudioService);

            _visualStudioService = visualStudioService;
            _fodyAddinManager = fodyAddinManager;
        }

        public bool IsAddinEnabledForProject(IProject project, IFodyAddin fodyAddin)
        {
            Argument.IsNotNull("project", project);
            Argument.IsNotNull("fodyAddin", fodyAddin);

            return project.Addins.Any(x => string.Equals(fodyAddin.Name, x.Name));
        }

        /// <summary>
        /// Loads the projects in the current solution.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISolutionItem> LoadProjects()
        {
            var projects = new List<ISolutionItem>();

            var vsProjects = _visualStudioService.GetAllProjects();
            foreach (var vsProject in vsProjects)
            {
                string solutionItemId = vsProject.UniqueName;

                string parentSolutionItemId = null;
                var parentSolutionItem = vsProject.GetParentProject();
                if (parentSolutionItem != null)
                {
                    parentSolutionItemId = parentSolutionItem.UniqueName;
                }

                var solutionItemName = vsProject.Name;

                if (vsProject.IsSolutionFolder())
                {
                    projects.Add(new SolutionFolder(solutionItemId, solutionItemName, parentSolutionItemId));
                }
                else if (!string.IsNullOrWhiteSpace(vsProject.FullName))
                {
                    string projectDirectory = Path.GetDirectoryName(vsProject.FullName);
                    var addins = GetAddins(projectDirectory);

                    projects.Add(new Project(solutionItemId, solutionItemName, projectDirectory, addins, parentSolutionItemId));
                }
            }

            return projects;
        }

        public bool SaveProjects(IEnumerable<IProject> projects)
        {
            Argument.IsNotNull("projects", projects);

            foreach (var project in projects)
            {
                if (!SaveProject(project))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<IFodyAddin> GetAddins(string projectDirectory)
        {
            Argument.IsNotNullOrWhitespace("projectDirectory", projectDirectory);

            var addins = new List<IFodyAddin>();

            var fileName = FodyHelper.GetFodyConfigForProject(projectDirectory);
            if (File.Exists(fileName))
            {
                var document = XDocument.Load(fileName);
                var weavers = document.Element("Weavers");
                foreach (var weaver in weavers.Elements())
                {
                    var addin = _fodyAddinManager.GetAddin(weaver.Name.LocalName);
                    if (addin != null)
                    {
                        addins.Add(addin);
                    }
                }
            }

            return addins;
        }

        private bool SaveProject(IProject project)
        {
            Argument.IsNotNull("project", project);

            try
            {
                XDocument config;
                var fodyConfig = FodyHelper.GetFodyConfigForProject(project.Directory);
                if (File.Exists(fodyConfig))
                {
                    config = XDocument.Load(fodyConfig);
                }
                else
                {
                    config = new XDocument();
                    config.Add(new XElement("Weavers"));
                }

                var root = config.Element("Weavers");
                root.RemoveAll();

                foreach (var addin in project.Addins)
                {
                    root.Add(new XElement(addin.Name));
                }

                config.Save(fodyConfig);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}