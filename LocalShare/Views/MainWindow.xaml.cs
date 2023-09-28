using LocalShare.ViewModels;
using ModernWpf.Controls;
using System.Windows.Input;

namespace LocalShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {



        public MainWindow()
        {
            InitializeComponent();
            this.FontFamily = new System.Windows.Media.FontFamily("Raleway");

        }




        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var selectedItem = args.SelectedItem as NavigationViewItem;

                var viewModel = DataContext as MainWindowViewModel;

                if (selectedItem != null && viewModel != null)
                {
                    string selectedTag = selectedItem.Tag.ToString();

                    switch (selectedTag)
                    {
                        case "DevicesOnline":
                            viewModel.NavigateToPage(selectedTag);
                            break;

                        case "Settings":
                            viewModel.NavigateToPage(selectedTag);
                            break;
                    }
                }
            }
        }


    }
}
