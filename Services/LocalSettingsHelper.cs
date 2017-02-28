using CostDaily.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CostDaily.Services
{
    public static class LocalSettingsHelper
    {
        private static SettingsContainer _defaultSettings {get; set;}
        private static ApplicationDataContainer _appSavedSettings { get; set; }

        static LocalSettingsHelper()
        {
            if (_defaultSettings == null) _defaultSettings = new SettingsContainer();
            if (_appSavedSettings == null) _appSavedSettings = ApplicationData.Current.LocalSettings;
        }

        public static string ReadAppSettings(SettingFields parameter)
        {
            //_appSavedSettings.Values.Clear();

            string setting = parameter.ToString();
            var value = _appSavedSettings.Values[setting];

            if (value == null)
            {
                value = returnDefaultSetting(setting);
                _appSavedSettings.Values[setting] = value;
            }

            return value.ToString();                    
        }

        public static void SaveRegionSettings(string value)
        {
            _appSavedSettings.Values[SettingFields.GeneralRegionAndLanguage.ToString()] = value;
        }

        private static string returnDefaultSetting(string setting)
        {
            string value;
            switch (setting)
            {
                case "GeneralRegionAndLanguage":
                    value = _defaultSettings.GeneralRegionAndLanguage;
                    break;
                case "CSVvalueStyle":
                    value = _defaultSettings.CSVvalueStyle;
                    break;
                default:
                    value = null;
                    break;
            }

            return value;
        }
    }
}
