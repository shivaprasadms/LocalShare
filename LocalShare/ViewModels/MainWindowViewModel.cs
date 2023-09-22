using LocalShare.Interfaces;

namespace LocalShare.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private INavigationService navigation;

        public INavigationService Navigation
        {
            get { return navigation; }
            set { navigation = value; RaisePropertyChanged(); }
        }



        public MainWindowViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;

            // DevicesOnlineCommand = new RelayCommand(o => Navigation.NavigateTo<DeviceOnlineViewModel>());

            //SettingsOnlineCommand = new RelayCommand(o => Navigation.NavigateTo<SettingsViewModel>());

            Navigation.NavigateTo<SettingsViewModel>();
        }

        public void NavigateToPage(string page)
        {
            switch (page)
            {
                case "DevicesOnline":
                    Navigation.NavigateTo<DeviceOnlineViewModel>();
                    break;

                case "Settings":
                    Navigation.NavigateTo<SettingsViewModel>();
                    break;

                case "About":
                    Navigation.NavigateTo<AboutViewModel>();
                    break;
            }
        }







    }
}
