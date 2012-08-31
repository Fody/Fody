using System.Windows;

namespace FodyVSPackage.Views
{
    using Catel.Windows;

    using ViewModels;

    /// <summary>
    /// Interaction logic for ManageAddinsView.xaml.
    /// </summary>
    public partial class ManageAddinsView : DataWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageAddinsView"/> class.
        /// </summary>
        public ManageAddinsView()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageAddinsView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to inject.</param>
        /// <remarks>
        /// This constructor can be used to use view-model injection.
        /// </remarks>
        public ManageAddinsView(ManageAddinsViewModel viewModel)
            : base(viewModel, DataWindowMode.Close)
        {
            StyleHelper.CreateStyleForwardersForDefaultStyles(Application.Current.Resources, Resources);

            InitializeComponent();
        }
    }
}
