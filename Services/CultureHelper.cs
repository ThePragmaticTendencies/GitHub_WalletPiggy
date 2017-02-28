using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Services
{
    public enum SupportedCultures
    {
        English_Euro = 0,
        English_Dollar,
        English_Pound,
        Polish_Złoty
    }

    public static class CultureHelper
    {

        public static CultureInfo ReturnCulture(string parameter)
        {
            CultureInfo requestedCulture;

            switch (parameter)
            {
                case "English_Euro":
                    requestedCulture = ReturnDefaultCulture();
                    break;
                case "English_Dollar":
                    requestedCulture = new CultureInfo("en-US");
                    break;
                case "English_Pound":
                    requestedCulture = new CultureInfo("en-GB");
                    break;
                case "Polish_Złoty":
                    requestedCulture = new CultureInfo("pl-PL");
                    break;
                default:
                    requestedCulture = ReturnDefaultCulture();
                    break;
            }

            return requestedCulture;
        }

        public static CultureInfo ReturnDefaultCulture()
        {

            CultureInfo requestedCulture = new CultureInfo("en-GB");
            CultureInfo basicCulture = new CultureInfo("de-DE");


            requestedCulture.NumberFormat = basicCulture.NumberFormat;

            return requestedCulture;
        }

        public static string ReturnRegion(CultureInfo culture)
        {
            string cultureName = culture.Name;
            string cultureDecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
            string region;

            switch (cultureName+"/"+ cultureDecimalSeparator)
            {
                case "en-GB/,":
                    region = "English_Euro";
                    break;
                case "en-US/.":
                    region = "English_Dollar";
                    break;
                case "en-GB/.":
                    region = "English_Pound";
                    break;
                case "pl-PL/,":
                    region = "Polish_Złoty";
                    break;
                default:
                    region = null;
                    break;
            }

            return region;
        }
    }
}
