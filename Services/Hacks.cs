using CostDaily.Common;
using CostDaily.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Services
{
    public static class Hacks
    {
        public static string CostToCSV(List<Cost> cost)
        {
            string objectString = null;
            string newLine = Environment.NewLine;
            string culture = null;
            string valueSeparator = Services.LocalSettingsHelper.ReadAppSettings(Common.SettingFields.CSVvalueStyle);

            if (App.AppCurrentRegionalInformation.Name.Contains("pl"))
            {
                culture = "pl-PL";
            }
            else
            {
                culture = "en";
            }

            foreach (Cost costElement in cost)
            {
                try
                {
                    objectString += App.LocalizationDictionaries[culture][costElement.CategoryName] + valueSeparator + costElement.Date.ToString() + valueSeparator + costElement.Value.ToString("C", App.AppCurrentRegionalInformation) + valueSeparator + newLine;
                }
                catch
                {
                    //objectString += costElement.CategoryName + valueSeparator + costElement.Date.ToString() + valueSeparator + costElement.Value.ToString("C", App.AppCurrentRegionalInformation) + valueSeparator + newLine;
                }
            }

            return objectString;
        }
    }
}
