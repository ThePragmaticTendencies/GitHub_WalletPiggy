using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace CostDaily.Common
{
    public class NotificationHelper : INotifyPropertyChanged
    {
        private string _message;
        private string _visibility;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
        public string Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public NotificationHelper(Visibility visibility, string message)
        {
            Message = message;
            Visibility = visibility.ToString();
        }

        public NotificationHelper(string message)
        {
            Message = message;
            Visibility = Windows.UI.Xaml.Visibility.Visible.ToString();
        }

        public NotificationHelper()
        {
            Message = "";
            Visibility = Windows.UI.Xaml.Visibility.Collapsed.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NotificationHelperExtended : NotificationHelper
    {
        private bool _successful;

        public bool Successful
        {
            get { return _successful; }
            set
            {
                _successful = value;
                OnPropertyChanged("Successful");

            }
        }

        public NotificationHelperExtended(string message) : base(message)
        {
            Successful = false;
        }

        public NotificationHelperExtended(string message, bool status) : base(message)
        {
            Successful = status;
        }

    }
}
