using Catel;
using Catel.Data;

namespace FodyVSPackage.Models
{
    public class FodyAddin : DataObjectBase<FodyAddin>, IFodyAddin
    {
        public FodyAddin(string name, string directory)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNullOrWhitespace("directory", directory);

            Name = name;
            Directory = directory;
        }

        /// <summary>
        /// Gets the name of the addin.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            private set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);

        /// <summary>
        /// Gets the directory name of the addin.
        /// </summary>
        public string Directory
        {
            get { return GetValue<string>(DirectoryProperty); }
            private set { SetValue(DirectoryProperty, value); }
        }

        /// <summary>
        /// Register the Directory property so it is known in the class.
        /// </summary>
        public static readonly PropertyData DirectoryProperty = RegisterProperty("Directory", typeof(string), string.Empty);
    }
}