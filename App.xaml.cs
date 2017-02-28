using CostDaily.Common;
using CostDaily.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Graphics.Display;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CostDaily
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        //My own solution
        //public static DataSource DataModel;

        public static DataSource DataModel;
        public static LocalizationSource LocalizationDataSource;
        public static Dictionary<string, Dictionary<string, string>> LocalizationDictionaries;

        public static CultureInfo AppCurrentRegionalInformation;
        public static CultureInfo StandardRegionalInformation = new CultureInfo("en-US");

        public static string StandardDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static string ShortDateFormat = "yyyy-MM-dd";
        public static string VeryShortDateFormat = "yy-MM-dd";
        public static string MonthDayDateFormat = "MM-dd";

        public static double ScaleFactor = 1.5;

        public static string[] SupportedCurrencies = new string[] { "pl-PL", "de-DE", "en-US", "en-GB" };
        public static string[] SupportedLanguages = new string[] { "pl-PL", "en" };
        
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            DataModel = new DataSource();
            LocalizationDataSource = new LocalizationSource();

            LocalizationDictionaries = new Dictionary<string, Dictionary<string, string>>();

            AppCurrentRegionalInformation = Services.CultureHelper.ReturnCulture(Services.LocalSettingsHelper.ReadAppSettings(SettingFields.GeneralRegionAndLanguage));

            ApplicationLanguages.PrimaryLanguageOverride = AppCurrentRegionalInformation.TwoLetterISOLanguageName;

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = Services.Navigation.GoBack();
        }

        bool IsDataLoaded()
        {
            return (DataModel.IsDataLoaded && LocalizationDataSource.IsDataLoaded);
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();

            LocalizationDictionaries = await LocalizationDataSource.GetDictionaries();

            if (!IsDataLoaded())
            {
                Window.Current.Content = new Splash_Extended();
            }
            else
            {
                DataModel.CheckTimeChanged();
                Services.Navigation.GotoMainPage();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

    }
}