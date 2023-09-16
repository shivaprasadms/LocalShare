using LocalShare.Interfaces;
using LocalShare.ViewModels;
using System;

namespace LocalShare.Services
{
    class NavigationService : ObservableObject, INavigationService
    {

        private ViewModel _currentView;

        private readonly Func<Type, ViewModel> viewModelFactory;

        public ViewModel CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; RaisePropertyChanged(); }
        }


        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            ViewModel viewModel = viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            this.viewModelFactory = viewModelFactory;

        }
    }
}
