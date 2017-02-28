using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CostDaily.Common
{
    public class KeyboardButton:INotifyPropertyChanged
    {
        private int _keyWidth;
        private int _keyHeight;
        public string Name { get; set; }
        public bool IsInputValue { get; set; }
        public ICommand ButtonClickCommand { get; set; }
        public int KeyWidth
        {
            get { return _keyWidth; }
            set
            {
                _keyWidth = value;
                OnPropertyChanged("KeyWidth");
            }
        }
        public int KeyHeight
        {
            get { return _keyHeight; }
            set
            {
                _keyHeight = value;
                OnPropertyChanged("KeyHeight");
            }
        }

        public KeyboardButton(string value, bool status)
        {
            Name = value;
            IsInputValue = status;
            KeyWidth = 0;
            KeyHeight = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    class Keyboard
    {
        private static List<KeyboardButton> _inputKeyboard = new List<KeyboardButton>();

        public static void PopulateKeyboard()
        {

            _inputKeyboard.Add(new KeyboardButton("7", true));
            _inputKeyboard.Add(new KeyboardButton("8", true));
            _inputKeyboard.Add(new KeyboardButton("9", true));
            _inputKeyboard.Add(new KeyboardButton("+", false));
            _inputKeyboard.Add(new KeyboardButton("4", true));
            _inputKeyboard.Add(new KeyboardButton("5", true));
            _inputKeyboard.Add(new KeyboardButton("6", true));
            _inputKeyboard.Add(new KeyboardButton("-", false));
            _inputKeyboard.Add(new KeyboardButton("1", true));
            _inputKeyboard.Add(new KeyboardButton("2", true));
            _inputKeyboard.Add(new KeyboardButton("3", true));
            _inputKeyboard.Add(new KeyboardButton("*", false));
            _inputKeyboard.Add(new KeyboardButton("+/-", true));
            _inputKeyboard.Add(new KeyboardButton("0", true));
            _inputKeyboard.Add(new KeyboardButton(".", true));
            _inputKeyboard.Add(new KeyboardButton("/", false));
            _inputKeyboard.Add(new KeyboardButton("ADD", false));
            _inputKeyboard.Add(new KeyboardButton("<-", true));
            _inputKeyboard.Add(new KeyboardButton("=", false));

        }


        public static List<KeyboardButton> GetKeyboardLayout()
        {
            if (_inputKeyboard.Count == 0)
            {
                PopulateKeyboard();
            }
            return _inputKeyboard;
        }
    }
}
