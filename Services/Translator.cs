using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Services
{
    public static class Translator
    {
        public static string Translate(string name)
        {
            string culture;

            if (App.AppCurrentRegionalInformation.Name.Contains("pl"))
            {
                culture = "pl-PL";
            }
            else
            {
                culture = "en";
            }

            string translation;
            try
            {
                translation = App.LocalizationDictionaries[culture][name];
            }
            catch
            {
                translation = name;
            }

            return translation;
        }
    }
}
