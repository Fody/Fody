using System.Collections.Generic;
using System.Linq;
using Catel;
using FodyVSPackage.ViewModels;

namespace FodyVSPackage
{
    public static class SelectableProjectHelper
    {
        public static IEnumerable<SelectableProject> ConvertFlatListToHierarchy(IEnumerable<SelectableProject> projects)
        {
            Argument.IsNotNull("projects", projects);

            Dictionary<string, SelectableProject> lookup = projects.ToDictionary(project => project.Id);
            var nested = new List<SelectableProject>();

            foreach (var project in projects)
            {
                if (project.ParentId == null)
                {
                    // Root project
                    nested.Add(project);
                }
                else
                {
                    lookup[project.ParentId].ChildProjects.Add(project);
                }
            }

            return nested;
        }

        public static IEnumerable<SelectableProject> ConvertHierarchyToFlatList(IEnumerable<SelectableProject> projects)
        {
            Argument.IsNotNull("projects", projects);

            var flatDictionary = new Dictionary<string, SelectableProject>();

            foreach (var project in projects)
            {
                if (!flatDictionary.ContainsKey(project.Id))
                {
                    flatDictionary.Add(project.Id, project);
                }

                foreach (var childProject in project.ChildProjects)
                {
                    foreach (var subProject in ConvertHierarchyToFlatList(new[] { childProject }))
                    {
                        if (!flatDictionary.ContainsKey(subProject.Id))
                        {
                            flatDictionary.Add(subProject.Id, subProject);
                        }
                    }                    
                }
            }

            return flatDictionary.Values;
        }
    }
}