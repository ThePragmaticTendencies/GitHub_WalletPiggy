using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;

namespace CostDaily.Common
{
    public enum SettingFields
    {
        GeneralRegionAndLanguage = 0,
        CSVvalueStyle
    }

    class SettingsContainer
    {
        public string GeneralRegionAndLanguage { get; set; }
        public string CSVvalueStyle { get; set; }

        public SettingsContainer()
        {
            if (ApplicationLanguages.PrimaryLanguageOverride.Contains("pl"))
            {
                GeneralRegionAndLanguage = Services.SupportedCultures.Polish_Złoty.ToString();
            }
            else
            {
                GeneralRegionAndLanguage = Services.SupportedCultures.English_Euro.ToString();
            }

            CSVvalueStyle = ";";
        }        
    }
}
