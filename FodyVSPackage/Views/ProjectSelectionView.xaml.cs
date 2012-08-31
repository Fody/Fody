using System.Windows;

namespace FodyVSPackage.Views
{
    using Catel.Windows;

    using ViewModels;

    /// <summary>
    /// Interaction logic for ProjectSelectionView.xaml.
    /// </summary>
    public partial class ProjectSelectionView : DataWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectionView"/> class.
        /// </summary>
        public ProjectSelectionView()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectionView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to inject.</param>
        /// <remarks>
        /// This constructor can be used to use view-model injection.
        /// </remarks>
        public ProjectSelectionView(ProjectSelectionViewModel viewModel)
            : base(viewModel)
        {
            StyleHelper.CreateStyleForwardersForDefaultStyles(Application.Current.Resources, Resources);

            InitializeComponent();
        }
    }
}
