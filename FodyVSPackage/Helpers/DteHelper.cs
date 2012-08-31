namespace FodyVSPackage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using EnvDTE;
    using EnvDTE80;

    public static class DteHelper
    {
        /// <summary>
        /// Cache for determining the relative name of project items.
        /// </summary>
        private static readonly Dictionary<string, string> _relativeNameCache = new Dictionary<string, string>();

        /// <summary>
        /// Determines whether visual studio has currently a solution opened.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <returns><c>true</c> if visual studio currently has a solution opened; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dte"/> is <c>null</c>.</exception>
        public static bool IsSolutionOpened(this DTE dte)
        {
            Argument.IsNotNull("dte", dte);

            return dte.GetActiveProject() != null;
        }

        public static Project GetActiveProject(this DTE dte)
        {
            Argument.IsNotNull("dte", dte);

            Project activeProject = null;

            var activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject;
        }

        public static Project GetProjectFromSolution(this Solution solution, string projectName, bool addProjectFolders = true)
        {
            Argument.IsNotNull("solution", solution);
            Argument.IsNotNullOrWhitespace("projectName", projectName);

            var allProjects = solution.GetAllProjects(addProjectFolders);

            return allProjects.FirstOrDefault(proj => proj.Name == projectName);
        }

        public static IList<Project> GetAllProjects(this Solution solution, bool addProjectFolders = true)
        {
            Argument.IsNotNull("solution", solution);

            Projects projects = solution.Projects;
            var list = new List<Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.IsSolutionFolder())
                {
                    var solutionFolderProjects = GetSolutionFolderProjects(project).ToList();

                    if (addProjectFolders && solutionFolderProjects.Any())
                    {
                        list.Add(project);
                    }

                    list.AddRange(solutionFolderProjects);
                }
                else
                {
                    list.Add(project);
                }
            }

            return list;
        }

        private static IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder, bool addProjectFolders = true)
        {
            var list = new List<Project>();
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.IsSolutionFolder())
                {
                    var solutionFolderProjects = GetSolutionFolderProjects(subProject).ToList();

                    if (addProjectFolders && solutionFolderProjects.Any())
                    {
                        list.Add(subProject);
                    }

                    list.AddRange(solutionFolderProjects);
                }
                else
                {
                    list.Add(subProject);
                }
            }

            return list;
        }

        public static Project GetParentProject(this Project project)
        {
            Argument.IsNotNull("project", project);

            var parentProjectItem = project.ParentProjectItem;
            while (parentProjectItem != null)
            {
                var parentProject = parentProjectItem as Project;
                if (parentProject != null)
                {
                    return parentProject;
                }

                return parentProjectItem.ContainingProject;
            }

            return null;
        }

        public static string GetProjectDirectory(this Project project)
        {
            return Path.GetDirectoryName(project.FileName);
        }

        /// <summary>
        /// Gets the name of the object. In case of <see cref="Project"/>, this method will return
        /// <see cref="Project.Name"/>. In case of <see cref="ProjectItem"/>, this method will return
        /// <see cref="ProjectItem.Name"/>. 
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The name of the object.</returns>
        /// <remarks>
        /// When this method is neither a <see cref="Project"/> nor a <see cref="ProjectItem"/>, this
        /// method will return <see cref="string.Empty"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        public static string GetObjectName(this object obj)
        {
            Argument.IsNotNull("obj", obj);

            var project = obj as Project;
            if (project != null)
            {
                return project.Name;
            }

            var projectItem = obj as ProjectItem;
            if (projectItem != null)
            {
                return projectItem.Name;
            }

            return String.Empty;
        }

        /// <summary>
        /// Gets the name of the project item relative to the root.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>The name of the item relative to the root.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="projectItem"/> is <c>null</c>.</exception>
        public static string GetNameRelativeToRoot(this ProjectItem projectItem)
        {
            Argument.IsNotNull("projectItem", projectItem);

            string fileName = projectItem.FileNames[0];
            if (!_relativeNameCache.ContainsKey(fileName))
            {
                string projectDirectory = GetProjectDirectory(projectItem.ContainingProject);
                string fullName = fileName.Replace(projectDirectory, String.Empty);

                while (fullName.EndsWith("\\"))
                {
                    fullName = fullName.Substring(0, fullName.Length - 1);
                }

                while (fullName.StartsWith("\\"))
                {
                    fullName = fullName.Substring(1);
                }

                _relativeNameCache.Add(fileName, fullName);
            }

            return _relativeNameCache[fileName];
        }

        /// <summary>
        /// Determines whether the specified project item is a resource (.resx) file.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///   <c>true</c> if the specified project item is a resource file; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="projectItem"/> is <c>null</c>.</exception>
        public static bool IsResourceFile(this ProjectItem projectItem)
        {
            Argument.IsNotNull("projectItem", projectItem);

            // Yep, this is right, we start at 1... :(
            for (int i = 1; i < projectItem.Properties.Count + 1; i++)
            {
                if (projectItem.Properties.Item(i).Name == "CustomTool")
                {
                    return (string)projectItem.Properties.Item(i).Value == "ResXFileCodeGenerator";
                }
            }

            return false;
        }

        public static bool IsSolutionFolder(this Project project)
        {
            Argument.IsNotNull("project", project);

            return project.Kind == ProjectKinds.vsProjectKindSolutionFolder;
        }

        /// <summary>
        /// Determines whether the specified project item is a folder.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///   <c>true</c> if the specified project item is a folder; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="projectItem"/> is <c>null</c>.</exception>
        public static bool IsFolder(this ProjectItem projectItem)
        {
            Argument.IsNotNull("projectItem", projectItem);

            return (projectItem.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}");
        }

        /// <summary>
        /// Determines whether the specified project item is a linked file.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///   <c>true</c> if the specified project item is a linked file; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method determines its value by checking whether the file physically exists on the disk.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="projectItem"/> is <c>null</c>.</exception>
        public static bool IsLinkedFile(this ProjectItem projectItem)
        {
            Argument.IsNotNull("projectItem", projectItem);

            if (projectItem.IsFolder())
            {
                return false;
            }

            return (bool)projectItem.Properties.Item("IsLink").Value;
        }

        /// <summary>
        /// Determines whether the specified project item is a xaml file.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///   <c>true</c> if the specified project item is a xaml file; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="projectItem"/> is <c>null</c>.</exception>
        public static bool IsXamlFile(this ProjectItem projectItem)
        {
            Argument.IsNotNull("projectItem", projectItem);

            return projectItem.FileNames[0].EndsWith(".xaml");
        }
    }
}