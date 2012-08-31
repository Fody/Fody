using System.Collections.Generic;
using System.Linq;
using Catel;
using Catel.Data;
using Catel.MVVM;
using FodyVSPackage.Models;
using FodyVSPackage.Services;

namespace FodyVSPackage.ViewModels
{
    public class ProjectSelectionViewModel : ViewModelBase
    {
        public ProjectSelectionViewModel(IFodyAddin fodyAddin)
        {
            Argument.IsNotNull("fodyAddin", fodyAddin);

            FodyAddin = fodyAddin;

            Projects = new List<SelectableProject>();

            var projectManager = GetService<IProjectManager>();
            var solutionItems = projectManager.LoadProjects();

            var flatProjects = new List<SelectableProject>();
            foreach (var solutionItem in solutionItems)
            {
                var project = solutionItem as IProject;
                var isSelected = (project != null) && projectManager.IsAddinEnabledForProject(project, fodyAddin);

                flatProjects.Add(new SelectableProject(solutionItem, isSelected));
            }

            Projects.AddRange(SelectableProjectHelper.ConvertFlatListToHierarchy(flatProjects));
        }

        public override string Title
        {
            get { return string.Format("Configure '{0}' package", FodyAddin.Name); }
        }

        /// <summary>
        /// Gets the addin.
        /// </summary>
        public IFodyAddin FodyAddin
        {
            get { return GetValue<IFodyAddin>(FodyAddinProperty); }
            private set { SetValue(FodyAddinProperty, value); }
        }

        /// <summary>
        /// Register the FodyAddin property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FodyAddinProperty = RegisterProperty("FodyAddin", typeof(IFodyAddin));

        /// <summary>
        /// Gets the list of available projects.
        /// </summary>
        public List<SelectableProject> Projects
        {
            get { return GetValue<List<SelectableProject>>(ProjectsProperty); }
            set { SetValue(ProjectsProperty, value); }
        }

        /// <summary>
        /// Register the Projects property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ProjectsProperty = RegisterProperty("Projects", typeof(List<SelectableProject>), null);

        protected override bool Save()
        {
            var flatProjects = SelectableProjectHelper.ConvertHierarchyToFlatList(Projects);
            var projectsToSave = new List<IProject>();

            // Remove projects that are no longer selected
            foreach (var selectableProject in flatProjects.Where(x => !x.IsSelected))
            {
                var project = selectableProject.SolutionItem as IProject;
                if (project == null)
                {
                    continue;
                }

                if (project.Addins.Contains(FodyAddin))
                {
                    project.Addins.Remove(FodyAddin);
                }

                projectsToSave.Add(project);
            }

            // Add projects that are now selected
            foreach (var selectableProject in flatProjects.Where(x => x.IsSelected))
            {
                var project = selectableProject.SolutionItem as IProject;
                if (project == null)
                {
                    continue;
                }

                if (!project.Addins.Contains(FodyAddin))
                {
                    project.Addins.Add(FodyAddin);
                }

                projectsToSave.Add(project);
            }

            var projectManager = GetService<IProjectManager>();
            return projectManager.SaveProjects(projectsToSave);
        }
    }

    public class SelectableProject : ObservableObject
    {
        private bool _isSelected;

        public SelectableProject(ISolutionItem solutionItem, bool isSelected)
        {
            Argument.IsNotNull("solutionItem", solutionItem);

            SolutionItem = solutionItem;

            ChildProjects = new List<SelectableProject>();

            Id = solutionItem.Id;
            ParentId = solutionItem.ParentId;
            Name = solutionItem.Name;
            IsSelected = isSelected;
        }

        public ISolutionItem SolutionItem { get; private set; }

        public string Id { get; private set; }

        public string ParentId { get; private set; }

        public string Name { get; private set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                foreach (var childProject in ChildProjects)
                {
                    childProject.IsSelected = _isSelected;
                }

                RaisePropertyChanged(() => IsSelected);
            }
        }

        public List<SelectableProject> ChildProjects { get; private set; }
    }
}