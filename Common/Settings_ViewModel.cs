using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;

namespace CostDaily.Common
{
    class Settings_ViewModel : INotifyPropertyChanged
    {
        private List<string> _regions;
        private string _isNotificationCollapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isSecondaryMenuAvailable { get; set; } = false;
        public string SelectedRegion { get; set; }

        public string IsNotificationCollapsed
        {
            get { return _isNotificationCollapsed; }
            set
            {
                _isNotificationCollapsed = value;
                OnPropertyChanged("IsNotificationCollapsed");
            }
        }

        public List<string> Regions
        {
            get { return _regions; }
            set { _regions = value; }
        }

        public bool IsSecondaryMenuAvailable
        {
            get { return _isSecondaryMenuAvailable; }
        }

        public Settings_ViewModel()
        {
            _regions = new List<string>();
            loadList();
            SelectedRegion = Services.LocalSettingsHelper.ReadAppSettings(SettingFields.GeneralRegionAndLanguage);
        }

        private void loadList()
        {
            if (_regions == null)
                return;

            var enumNames = Enum.GetNames(typeof(Services.SupportedCultures));

            foreach (string elem in enumNames)
            {
                _regions.Add(elem);
            }
        }

        private string _normalizeEnum(Services.SupportedCultures region)
        {
            return region.ToString().Replace("_", " / ");
        }

        private string _normalizeEnumString(string region)
        {
            return region.Replace("_", " / ");
        }

        public async void SendCSVviaEmail()
        {
            var file = await App.DataModel.GetFileCSV();
            sendFileviaEMail(file);
        }

        public async void SendJSONviaEmail()
        {
            var file = await App.DataModel.GetFileJSON();
            sendFileviaEMail(file);
        }

        private async void sendFileviaEMail(StorageFile file)
        {
            EmailMessage email = new EmailMessage();
            email.Attachments.Add(new EmailAttachment(file.Name, file));
            email.Subject = "Wallet-Piggy: " + file.Name.ToString();
            await EmailManager.ShowComposeNewEmailAsync(email);
        }

        public void ClearDataBase()
        {

        }

        public void SaveNewSetting()
        {
            Services.LocalSettingsHelper.SaveRegionSettings(SelectedRegion);
        }

        public void DoesLanguageChangeEffects()
        {
            if (Services.CultureHelper.ReturnRegion(App.AppCurrentRegionalInformation).Contains(SelectedRegion))
            {
                IsNotificationCollapsed = "Collapsed";
            }
            else
            {
                IsNotificationCollapsed = "Visible";
            }
        }


    }
}
