using LocalShare.Configuration;
using System;
using System.Configuration;

namespace LocalShare.Models
{
    internal class SettingsModel
    {

        private AppConfiguration _appConfig;

        public SettingsModel(AppConfiguration appconfig)
        {
            _appConfig = appconfig;
        }


        public void ChangeSavePath(string path)
        {
            ChangeSetting(nameof(_appConfig.LocalShareSavePath), path);
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
