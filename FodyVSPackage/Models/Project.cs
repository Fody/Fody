using System.Collections.Generic;

namespace FodyVSPackage.Models
{
    using Catel.Data;

    /// <summary>
    /// Project Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class Project : SolutionItem, IProject
    {
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Project(string id, string name, string directory, IEnumerable<IFodyAddin> addins, string parentId = null)
            : base(id, name, parentId)
        {
            Directory = directory;

            Addins = new List<IFodyAddin>(addins);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the project directory.
        /// </summary>
        public string Directory
        {
            get { return GetValue<string>(DirectoryProperty); }
            set { SetValue(DirectoryProperty, value); }
        }

        /// <summary>
        /// Register the Directory property so it is known in the class.
        /// </summary>
        public static readonly PropertyData DirectoryProperty = RegisterProperty("Directory", typeof(string), string.Empty);

        /// <summary>
        /// Gets the list of addins for this project.
        /// </summary>
        public List<IFodyAddin> Addins
        {
            get { return GetValue<List<IFodyAddin>>(AddinsProperty); }
            set { SetValue(AddinsProperty, value); }
        }

        /// <summary>
        /// Register the Addins property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AddinsProperty = RegisterProperty("Addins", typeof(List<IFodyAddin>), null);

        public string FodyConfig
        {
            get { return FodyHelper.GetFodyConfigForProject(Directory); }
        }
        #endregion
    }
}