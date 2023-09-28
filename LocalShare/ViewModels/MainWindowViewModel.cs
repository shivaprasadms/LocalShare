using LocalShare.Interfaces;

namespace LocalShare.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get { return _navigation; }
            set { SetProperty(ref _navigation, value, nameof(Navigation)); }
        }


        public MainWindowViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;
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
