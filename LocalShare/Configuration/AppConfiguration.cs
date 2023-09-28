using System;
using System.Configuration;
using System.IO;

namespace LocalShare.Configuration
{
    public class AppConfiguration
    {

        public string LocalShareSavePath { get; private set; }
        public bool IsRunAtStartupEnabled { get; private set; }
        public bool IsMinimizeToTrayEnabled { get; private set; }

        public AppConfiguration()
        {
            LocalShareSavePath = ConfigurationManager.AppSettings["LocalShareSavePath"] ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LocalShare");
            IsRunAtStartupEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["IsRunAtStartupEnabled"]);
            IsMinimizeToTrayEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["IsMinimizeToTrayEnabled"]);
        }

        public void ChangeSavePath(string path)
        {
            LocalShareSavePath = path;
            ChangeSetting(nameof(LocalShareSavePath), path);
        }

        public void EnableRunAtStartup(bool value)
        {
            IsRunAtStartupEnabled = value;
            ChangeSetting(nameof(IsRunAtStartupEnabled), value.ToString());
        }

        public void EnableMinimizeToTray(bool value)
        {
            IsMinimizeToTrayEnabled = value;
            ChangeSetting(nameof(IsMinimizeToTrayEnabled), value.ToString());
        }

        private void ChangeSetting(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;

                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }



    }
}
