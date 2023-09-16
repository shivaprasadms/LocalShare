using LocalShare.Interfaces;
using LocalShare.Utility;

namespace LocalShare.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private INavigationService navigation;

        public INavigationService Navigation
        {
            get { return navigation; }
            set { navigation = value; RaisePropertyChanged(); }
        }

        public RelayCommand DevicesOnlineCommand { get; set; }

        public RelayCommand SettingsOnlineCommand { get; set; }

        public MainWindowViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;

            DevicesOnlineCommand = new RelayCommand(o => Navigation.NavigateTo<DeviceOnlineViewModel>());

            SettingsOnlineCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>(); });

            Navigation.NavigateTo<DeviceOnlineViewModel>();
        }






    }
}
