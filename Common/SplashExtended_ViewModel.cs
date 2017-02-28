using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CostDaily.Common
{
    class SplashExtended_ViewModel : INotifyPropertyChanged
    {
        private NotificationHelperExtended _notification { get; set; }
        private string _message;
        private bool _status;
        private bool _menuVisible;


        private SolidColorBrush _progressColor;
        public SolidColorBrush ProgressColor
        {
            get { return _progressColor; }

            set
            {
                _progressColor = value;
                OnPropertyChanged("ProgressColor");
            }
        }

        public bool MenuVisible
        {
            set
            {
                _menuVisible = value;
                OnPropertyChanged("MenuVisible");
                OnPropertyChanged("Menu_BackUpVisible");
            }
            get
            {
                return _menuVisible;
            }
        }

        public bool Menu_BackUpVisible
        {
            get
            {
                return !_menuVisible;
            }
        }

        public string Message
        {
            get { return _message; }

            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
        public bool Status
        {
            set
            {
                _status = value;
                OnPropertyChanged("StatusOK");
                OnPropertyChanged("StatusNOOK");        
            }
        }
        public bool StatusOK
        {
            get { return _status; }
        }
        public bool StatusNOOK
        {
            get
            {
                    return !_status;
            }
        }
        public NotificationHelperExtended Notification
        {
            get { return _notification; }

            set
            {
                _notification = value;
                OnPropertyChanged("Notification");
            }
        }

        public SplashExtended_ViewModel()
        {
            AppLoadProgressColor();
            _menuVisible = true;
        }
      
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler NotificationPop;
        public void OnNotificationPopped(EventArgs e)
        {
            EventHandler handler = NotificationPop;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public async Task LoadAppData(bool loadBackUp)
        {
            if (!loadBackUp)
            {
                await App.DataModel.LoadData();
            }
            else
            {
                Notification = await App.DataModel.LoadBackUp();
                Message = Notification.Message;
                Status = Notification.Successful;
                OnNotificationPopped(null);
                await Task.Delay(1000);
            }
        }

        public void BackupLoadProgressColor()
        {
            Color darkRedBrush = (Color)Application.Current.Resources["Piggy_DarkRed"];
            ProgressColor = new SolidColorBrush(darkRedBrush);
        }

        public void AppLoadProgressColor()
        {
            Color greyBrush = (Color)Application.Current.Resources["Piggy_ShadowGrey"];
            ProgressColor = new SolidColorBrush(greyBrush);
        }

        public void DisplayNewMessage(string message)
        {
            Notification = new NotificationHelperExtended(message);
            Message = Notification.Message;
            Status = Notification.Successful;

            OnNotificationPopped(null);
        }

        public void SwitchOffMenu()
        {
            if (MenuVisible) MenuVisible = false;
        }

        public void SwitchOnMenu()
        {
            if (!MenuVisible) MenuVisible = true;
        }
    }
}
