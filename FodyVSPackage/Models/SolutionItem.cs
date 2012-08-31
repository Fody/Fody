using Catel.Data;

namespace FodyVSPackage.Models
{
    public class SolutionItem : DataObjectBase<SolutionItem>, ISolutionItem
    {
        public SolutionItem(string id, string name, string parentId = null)
        {
            Id = id;
            ParentId = parentId;
            Name = name;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id
        {
            get { return GetValue<string>(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        /// <summary>
        /// Register the Id property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IdProperty = RegisterProperty("Id", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        public string ParentId
        {
            get { return GetValue<string>(ParentIdProperty); }
            set { SetValue(ParentIdProperty, value); }
        }

        /// <summary>
        /// Register the ParentId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ParentIdProperty = RegisterProperty("ParentId", typeof(string), null);

        /// <summary>
        /// Gets or sets the name of the root project.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);
    }
}