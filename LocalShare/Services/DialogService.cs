using LocalShare.Interfaces;
using System.Windows;

namespace LocalShare.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message)
        {

            MessageBox.Show(message);

            //new ContentDialog()
            //{
            //    Title = "LocalShare",
            //    Content = message,
            //    PrimaryButtonText = "OK",
            //}.ShowAsync();
        }
    }
}
