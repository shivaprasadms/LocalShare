using LocalShare.ViewModels;

namespace LocalShare.Interfaces
{
    public interface INavigationService
    {
        ViewModel CurrentView { get; }

        void NavigateTo<T>() where T : ViewModel;
    }
}