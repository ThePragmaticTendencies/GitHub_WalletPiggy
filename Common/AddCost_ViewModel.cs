using System;
using CostDaily.DataModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Collections.ObjectModel;
using CostDaily.Converters;

namespace CostDaily.Common
{    
   
    public class AddCost_ViewModel : INotifyPropertyChanged
    {
        private enum ValidatorMessage
        {
            MaxOutputLength=0,
            MaxDecimalLength,
            MaxValue,
            Negative,
            MaxInteger
        }
        private Action<string> Validators;

        private double _notificationsBlockTranslationY;
        public bool IsNotificationShown { get; set; } = false;
        private bool _isNotificationRaised;
        public bool IsNotificationRaised
        {
            get { return _isNotificationRaised; }
            set
            {
                _isNotificationRaised = value;
            }
        }

        public double NotificationsBlockTranslationY
        {
            get { return _notificationsBlockTranslationY; }
            set
            {
                _notificationsBlockTranslationY = value;
                OnPropertyChanged("NotificationsBlockTranslationY");
            }
        }

        private const double _upperLimit = 9999999.99d;
        public static List<string> Currencies { get; set; }

        private List<KeyboardButton> _keyboardLayout;
        private bool _isformEnebled = true;
        private bool _isValid = true;
        private bool _notifyIco = false;
        public ObservableCollection<NotificationHelperExtended> Notifications { get; set; }
        private NotificationHelperExtended _notification;

        public bool NotifyIco
        {
            get { return _notifyIco; }
            set
            {
                _notifyIco = value;
                OnPropertyChanged("NotifyIco");

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

        public bool IsFormEnebled
        {
            get { return _isformEnebled; }
            set
            {
                _isformEnebled = value;
                OnPropertyChanged("IsFormEnebled");
                OnFormEnebled(null);
            }

        }

        private Calculator _calculator = new Calculator();
        public List<KeyboardButton> KeyboardLayout
        {
            get
            {
                if (_keyboardLayout == null)
                {
                    _keyboardLayout = Keyboard.GetKeyboardLayout();
                    return _keyboardLayout;
                }
                else
                {
                    return _keyboardLayout;
                }
            }

            set { _keyboardLayout = value; }
        }


        private string _calculatorTextBlock = "0";

        private DateTimeOffset _costDate = DateTimeOffset.Now;
        private string _consoleCurrency = App.AppCurrentRegionalInformation.NumberFormat.CurrencySymbol;
        public string ConsoleCurrency
        {
            get { return _consoleCurrency; }
        }

        public string CalculatorTextBlock
        {
            get { return _calculatorTextBlock; }
            set
            {
                    _calculatorTextBlock = value;
                    OnPropertyChanged("CalculatorTextBlock");
            }
        }

        public DateTimeOffset CostDate
            {
            get { return _costDate; }
            set
            {
                _costDate = value;
                OnPropertyChanged("CostDate");
            }
            }
        private Cost _costTemplate;
        public Cost CostTemplate
        {
            get {return _costTemplate;}
            set {_costTemplate = value;}
        }

        private RelayCommand _calcButtonClick;
        public RelayCommand CalcButtonClick
        {
            get
            { return this._calcButtonClick; }
        }
        
        public void PressedCommand(object parameter)
        {
            KeyboardButton pressedButton = (KeyboardButton)parameter;

            if (pressedButton.Name == "ADD")
            {
                OnFormCompleted(new EventArgs());
            }
            else
            {
                CalculatorTextBlock = Validate(_calculator.ExecuteCalculator(pressedButton));
            }
        }
        
        public AddCost_ViewModel()
        {
            _calculator = new Calculator();
            Notifications = new ObservableCollection<NotificationHelperExtended>();
            Notifications.CollectionChanged += (s, e) =>
            {
                IsNotificationRaised = (Notifications.Count != 0) ? true : false;
            };
            Currencies = new List<string>();
            NotificationsBlockTranslationY = -25;
            this._calcButtonClick = new RelayCommand(this.PressedCommand);
            this.KeyboardLayout = _calculator.KeyboardLayout;
            Validators += ValidateMaxDecimalLength;
            Validators += ValidateMaxOutputLength;
            Validators += ValidateMaxValue;
            Validators += ValidateNegative;
            Validators += ValidateMaxIntegerLength;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler FormCompleted;
        protected void OnFormCompleted(EventArgs e)
        {
            //bubble the event up to the parent
            if (this.FormCompleted != null)
                this.FormCompleted(this, e);
        }

        public event EventHandler Translated;
        protected virtual void OnTranslated(EventArgs e)
        {
            EventHandler handler = Translated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Sized;
        protected virtual void OnSized(EventArgs e)
        {
            EventHandler handler = Sized;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler NotificationRaised;
        protected virtual void OnNotificationRaised(EventArgs e)
        {
            EventHandler handler = NotificationRaised;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler FormEnebled;
        protected virtual void OnFormEnebled(EventArgs e)
        {
            EventHandler handler = FormEnebled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Cost ReturnEditedCost()
        {
            this._costTemplate.Value = Convert.ToDecimal(this.CalculatorTextBlock, App.StandardRegionalInformation);
            this._costTemplate.Date = CostDate.ToString(App.StandardDateTimeFormat, App.StandardRegionalInformation);
            return this.CostTemplate;
        }
        public void UpdateKeyboardButtonSize(int displaySize)
        {
                double cropFactor = 0.5d;
                double columnsNumber = 4.0;
                double sizeFactor = 0.30d;
                double inputKeyCrop = 3 / 5.0;

                int maxWidth = (int)Math.Floor(displaySize * cropFactor);
                int keyWidth = (int)Math.Floor(maxWidth / (columnsNumber - (1 - inputKeyCrop)));
                int keyHeight = (int)Math.Floor(keyWidth * sizeFactor);

                foreach (KeyboardButton KeyButton in KeyboardLayout)
                {
                    KeyButton.KeyWidth = keyWidth;
                    KeyButton.KeyHeight = keyHeight;

                    if (KeyButton.IsInputValue == false)
                    {
                        if (KeyButton.Name == "ADD")
                        {
                            KeyButton.KeyWidth = keyWidth * 2;
                        }
                        else
                        {
                            KeyButton.KeyWidth = (int)Math.Floor(keyWidth * inputKeyCrop);
                        }
                    }
                }
        }

        public void SetNotificationsBlockTranslation(double blockHeight)
        {
            if (IsNotificationShown)
            {
                if (Notifications.Count == 0)
                {
                    NotificationsBlockTranslationY = -blockHeight;
                    IsNotificationShown = false;
                }
                else
                {
                    NotificationsBlockTranslationY = 0;
                }
            }
            else
            {
                NotificationsBlockTranslationY = -blockHeight;
            }
        }

        public void NotificationsBlockTranslation(double blockHeight)
        {
            NotificationsBlockTranslationY = _calcBlockTranslation(blockHeight);
            OnTranslated(null);
        }

        public void NotificationsBlockSizeTranslation(double blockHeight)
        {
            NotificationsBlockTranslationY = _calcBlockTranslation(blockHeight);
            OnSized(null);
        }

        private double _calcBlockTranslation(double blockHeight)
        {
            if (IsNotificationShown)
            {
                if (Notifications.Count == 0)
                {
                    IsNotificationShown = false;
                    return -blockHeight;                    
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return -blockHeight;
            }
        }


        //ALE SZKARADA!!!!!!
        public void PopulateCalcGrid(FrameworkElement _grid)
        {
            Grid calculatorGrid = (Grid)_grid;
            Button calcButton;
            int keyNumber = 0;

            for (int row = 0; row < calculatorGrid.RowDefinitions.Count; row++)
            {
                for (int column = 0; column < calculatorGrid.ColumnDefinitions.Count; column++)
                {

                    calcButton = new Button();
                    calcButton.SetValue(Grid.ColumnProperty, column);
                    calcButton.SetValue(Grid.RowProperty, row);
                    calcButton.Style = (Style)Application.Current.Resources["CustomButtonStyle"];

                    Color piggyShit = (Color)Application.Current.Resources["Piggy_DarkRed"];

                    if (KeyboardLayout[keyNumber].IsInputValue == false)
                    {
                        calcButton.Foreground = new SolidColorBrush(piggyShit);
                        calcButton.FontWeight = Windows.UI.Text.FontWeights.Bold;
                    }

                    Binding calcButtonContentBinding = new Binding
                    {
                        Source = KeyboardLayout[keyNumber],
                        Path = new PropertyPath("Name"),
                        Converter = new DecimalPointRegionalInformationConverter()
                    };
                    Binding calcButtonCommandBinding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("CalcButtonClick")
                    };
                    Binding calcButtonCommandParameterBinding = new Binding
                    {
                        Source = KeyboardLayout[keyNumber],
                    };

                    calcButton.SetBinding(ContentControl.ContentProperty, calcButtonContentBinding);
                    calcButton.SetBinding(Button.CommandProperty, calcButtonCommandBinding);
                    calcButton.SetBinding(Button.CommandParameterProperty, calcButtonCommandParameterBinding);

                    if (column == 0 && row == 4)
                    {
                        Binding calcButtonEnebledBinding = new Binding
                        {
                            Source = this,
                            Path = new PropertyPath("IsFormEnebled")
                        };

                        calcButton.SetBinding(Button.IsEnabledProperty, calcButtonEnebledBinding);

                        calcButton.SetValue(Grid.ColumnSpanProperty, 2);
                        calcButton.FontSize = 48;
                        column++;
                    }

                    calculatorGrid.Children.Add(calcButton);
                    keyNumber++;
                }
            }
        }



        private string Validate(string result)
        {
            if (Notifications != null) Notifications.Clear();
            _isValid = true;
            NotifyIco = false;

            if (result != "0.")
            {
                Validators(result);
            }

            OnNotificationRaised(null);

            if (_isValid)
            {
                enebleForm();
            }
            else
            {
                disableForm();
            }

            return result;

        }

        private void ValidateNegative(string result)
        {
            if (isNegative(result))
            {
                if(_isValid) _isValid = false;
                AddNotification(CreateNotification(ValidatorMessage.Negative));
            }
        }

        private void ValidateMaxValue(string result)
        {
            double value = Convert.ToDouble(result, App.StandardRegionalInformation);

            if (value > _upperLimit)
            {
                if (_isValid) _isValid = false;
                AddNotification(CreateNotification(ValidatorMessage.MaxValue));
            }
        }

        private void ValidateMaxOutputLength(string result)
        {
            if (isMaxLength(result, _calculator.MaxOutputLength))
            {
                if (!NotifyIco) NotifyIco = true;
                AddNotification(CreateNotification(ValidatorMessage.MaxOutputLength));
            }
        }

        private void ValidateMaxDecimalLength(string result)
        {
            if (isMaxDecimal(result, _calculator.MaxDecimal))
            {
                if (!NotifyIco) NotifyIco = true;
                AddNotification(CreateNotification(ValidatorMessage.MaxDecimalLength));
            }
        }

        private void ValidateMaxIntegerLength(string result)
        {
            if (isInteger(result))
            {
                if (isMaxInteger(result, _calculator.MaxOutputLength, _calculator.MaxDecimal))
                {
                    if (!NotifyIco) NotifyIco = true;
                    AddNotification(CreateNotification(ValidatorMessage.MaxInteger));
                }
            }
        }

        private bool isMaxDecimal(string result, int maxDecimal)
        {
            if (!isInteger(result))
            {
                int decimalLength = result.Count() - result.IndexOf(".") - 1;

                return (decimalLength >= maxDecimal);
            }
            return false;
        }

        private bool isInteger(string result)
        {
            return (!result.Contains("."));
        }

        private bool isMaxInteger(string result, int maxLength, int maxDecimal)
        {
            int maxIntegerLength = maxLength - (maxDecimal + 1);

            maxIntegerLength += (isNegative(result)) ? 1 : 0;

            int length = result.Length;
            int integerLength = (isInteger(result)) ? length : result.IndexOf(".");

            return (integerLength >= maxIntegerLength);
        }

        private bool isNegative(string result)
        {
            return (result.Contains("-"));
        }

        private bool isMaxLength(string result, int maxLength)
        {
            return (isNegative(result)) ? (result.Length > maxLength) : (result.Length >= maxLength);
        }

        private void disableForm()
        {
            if (this.IsFormEnebled == true) this.IsFormEnebled = false;
        }

        private void enebleForm()
        {
            if (this.IsFormEnebled == false) this.IsFormEnebled = true;
        }

        private NotificationHelperExtended CreateNotification(ValidatorMessage message)
        {
            return new NotificationHelperExtended(message.ToString());
        }

        public void AddNotification(NotificationHelperExtended notification)
        {
            if (Notifications != null)
            {
                NotificationHelperExtended tempRef = Notifications.FirstOrDefault(p => p.Message.Contains(notification.Message)) as NotificationHelperExtended;

                if (tempRef != null)
                {
                    Notifications.Remove(tempRef);
                }

                Notifications.Add(notification);
            }
            else
            {
                Notifications = new ObservableCollection<NotificationHelperExtended>();
                Notifications.Add(notification);
            }
        }
    }

}