using LocalShare.Configuration;
using LocalShare.Utility;
using Microsoft.Win32;

namespace LocalShare.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private readonly AppConfiguration _appConfig;

        public RelayCommand ChangeSavePathCommand { get; private set; }

        public SettingsViewModel(AppConfiguration appSettings)
        {
            _appConfig = appSettings;
            ChangeSavePathCommand = new RelayCommand(o => ChangeFileSavePath());
        }

        public string SavePath
        {
            get { return _appConfig.LocalShareSavePath; }

            set
            {
                if (_appConfig.LocalShareSavePath != value)
                {
                    _appConfig.ChangeSavePath(value);
                    RaisePropertyChanged(nameof(SavePath));
                }
            }
        }

        public bool RunAtStartup
        {
            get { return _appConfig.IsRunAtStartupEnabled; }

            set
            {
                LaunchAppAtStartup(value);
                _appConfig.EnableRunAtStartup(value);
                RaisePropertyChanged(nameof(RunAtStartup));

            }
        }

        public bool MinimizeToTray
        {
            get { return _appConfig.IsMinimizeToTrayEnabled; }

            set
            {
                _appConfig.EnableMinimizeToTray(value);
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

        private void LaunchAppAtStartup(bool value)
        {
            RegistryKey? regKey = Registry.CurrentUser.OpenSubKey
                             ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (value)
            {
                regKey?.SetValue("LocalShare", System.Environment.ProcessPath);
            }
            else
            {
                regKey?.DeleteValue("localShare", false);
            }


        }





    }
}
