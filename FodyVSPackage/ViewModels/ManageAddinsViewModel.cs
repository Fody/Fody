using System.Collections.ObjectModel;
using Catel;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Catel.MVVM.Services;
using FodyVSPackage.Models;
using FodyVSPackage.Services;

namespace FodyVSPackage.ViewModels
{
    public class ManageAddinsViewModel : ViewModelBase
    {
        private readonly IFodyAddinManager _addinManager;
        private readonly IUIVisualizerService _uiVisualizerService;

        public ManageAddinsViewModel(IFodyAddinManager addinManager, IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull("addinManager", addinManager);

            _addinManager = addinManager;
            _uiVisualizerService = uiVisualizerService;

            Go = new Command<IFodyAddin>(OnGoExecute);
            Refresh = new Command(OnRefreshExecute);
        }

        public override string Title
        {
            get { return "Select the Fody addin to manage"; }
        }

        /// <summary>
        /// Gets the available addins.
        /// </summary>
        public ObservableCollection<IFodyAddin> AvailableAddins
        {
            get { return GetValue<ObservableCollection<IFodyAddin>>(AvailableAddinsProperty); }
            private set { SetValue(AvailableAddinsProperty, value); }
        }

        /// <summary>
        /// Register the AvailableAddins property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AvailableAddinsProperty = RegisterProperty("AvailableAddins", typeof(ObservableCollection<IFodyAddin>),
            () => new ObservableCollection<IFodyAddin>());

        /// <summary>
        /// Gets the Go command.
        /// </summary>
        public Command<IFodyAddin> Go { get; private set; }

        /// <summary>
        /// Method to invoke when the Go command is executed.
        /// </summary>
        private void OnGoExecute(IFodyAddin fodyAddin)
        {
            var vm = new ProjectSelectionViewModel(fodyAddin);
            _uiVisualizerService.ShowDialog(vm);
        }

        /// <summary>
        /// Gets the Refresh command.
        /// </summary>
        public Command Refresh { get; private set; }

        /// <summary>
        /// Method to invoke when the Refresh command is executed.
        /// </summary>
        private void OnRefreshExecute()
        {
            _addinManager.ReloadAddins();

            var addins = _addinManager.GetAddins();
            AvailableAddins = new ObservableCollection<IFodyAddin>(addins);
        }

        protected override void Initialize()
        {
            AvailableAddins.AddRange(_addinManager.GetAddins());
        }
    }
}