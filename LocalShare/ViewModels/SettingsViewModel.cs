using LocalShare.Configuration;
using LocalShare.Utility;

namespace LocalShare.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private readonly AppSettingsManager _appSettings;

        public RelayCommand ChangeSavePathCommand { get; private set; }

        public SettingsViewModel(AppSettingsManager appSettings)
        {
            _appSettings = appSettings;
            ChangeSavePathCommand = new RelayCommand(o => ChangeFileSavePath());
        }

        public string SavePath
        {
            get { return _appSettings.LocalShareSavePath; }

            set
            {
                if (_appSettings.LocalShareSavePath != value)
                {
                    _appSettings.ChangeSavePath(value);
                    RaisePropertyChanged(nameof(SavePath));
                }
            }
        }

        public bool RunAtStartup
        {
            get { return _appSettings.IsRunAtStartupEnabled; }

            set
            {
                _appSettings.EnableRunAtStartup(value);
                RaisePropertyChanged(nameof(RunAtStartup));

            }
        }

        public bool MinimizeToTray
        {
            get { return _appSettings.IsMinimizeToTrayEnabled; }

            set
            {
                _appSettings.EnableMinimizeToTray(value);
                RaisePropertyChanged(nameof(MinimizeToTray));

            }
        }

        private void ChangeFileSavePath()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    SavePath = fbd.SelectedPath;

                }
            }
        }





    }
}
