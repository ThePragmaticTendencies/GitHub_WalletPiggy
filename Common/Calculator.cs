using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Common
{
    class Calculator
    {
        private bool _viewIsInInput = false;
        private int _maxOutputLength = 10;
        private int _maxDecimal = 2;
        private double _upperLimit = 9999999.99d;

        public int MaxOutputLength
        {
            get { return _maxOutputLength; }
        }

        public int MaxDecimal
        {
            get { return _maxDecimal; }
        }

        public double MaxValue
        {
            get { return _upperLimit; }
        }

        private string _output;

        private List<KeyboardButton> _keyboardLayout;
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

        public Calculator()
        {
            _output = "0";
            _keyboardLayout = Keyboard.GetKeyboardLayout();
        }

        public string ExecuteCalculator(KeyboardButton pressedButton)
        {
            if (pressedButton.IsInputValue)
            {
                this.modifyOutput(pressedButton.Name);
            }
            else
            {
                if (this.isViewInInput)
                {
                    this.resetInputView();
                    SingleEquation.UpdateEquation(this._output, pressedButton.Name);
                    this._output = SingleEquation.GetRoundedResult(2);
                }
                else
                {
                    SingleEquation.Operator = pressedButton.Name;
                }
                this.resetInputView();
            }

            return _output;
        }

        private void modifyOutput(string parameter)
        {
            switch (parameter)
            {
                case ".":
                    this.addDecimalPoint();
                    break;
                case "+/-":
                    this.changeOutputSign();
                    break;
                case "<-":
                    this.deleteLastInput();
                    break;
                default:
                    this._setOutput(parameter);
                    this.setInputView();
                    break;
            }
        }

        private void addDecimalPoint()
        {
            if (!this.isViewInInput || isZero())
            {
                this._setOutput("0.");
                this.setInputView();
            }
            else if (this.isViewInteger())
            {
                this._setOutput(".");
            }
        }

        private void deleteLastInput()
        {
            resetInputView();
            if (!isZero())
            {
                string consoleValue = this._output;
                if (consoleValue.Length == 1 || (consoleValue.Length == 2 && consoleValue.Contains("-")))
                {
                    this._setOutput("0");
                }
                else
                {
                    this._setOutput(consoleValue.Remove(consoleValue.Length - 1));
                    this.setInputView();
                }
            }
        }

        private void changeOutputSign()
        {
            this.resetInputView();
            if (this._output.Contains("-"))
            {
                this._setOutput(this._output.Substring(1));
            }
            else if (!isZero())
            {
                this._setOutput(String.Concat("-", this._output));
            }

            this.setInputView();
        }

        private bool isViewInInput
        {
            get { return _viewIsInInput; }
        }

        private void setInputView()
        {
            _viewIsInInput = true;
        }

        private void resetInputView()
        {
            _viewIsInInput = false;
        }

        private bool isViewInteger()
        {
            return isInteger(_output);
        }

        private bool isInteger(string value)
        {
            return !value.Contains(".");
        }

        private bool isZero()
        {
            return (_output == "0");
        }

        private void _setOutput(string value)
        {
            int length = _output.Length;

            if (_viewIsInInput && !isZero())
            {
                string result = _output + value;

                if (!IsMaxLength(result))
                {
                    _output += value;
                }
                else
                {
                    _output = replaceLastChar(length, value);
                }
            }

            else
            {
                _output = value;
            }
        }

        private string replaceLastChar(int length, string value)
        {
            return _output.Remove(length - 1) + value;

        }
        public bool IsMaxDecimal(string result)
        {
            if (result.Contains("."))
            {
                int decimalLength = result.Length - result.IndexOf(".");

                return (decimalLength > _maxDecimal + 1);
            }
            return false;
        }

        public bool IsMaxLength(string result)
        {

            if (!IsMaxInteger(result))
            {
                return IsMaxDecimal(result);
            }
            else
            {
                return true;
            }
        }

        public bool IsNegative(string result)
        {
            return result.Contains("-");
        }

        public bool IsMaxInteger(string result)
        {

            int maxIntegerLength = _maxOutputLength - (_maxDecimal + 1);

            maxIntegerLength += (IsNegative(result)) ? 1 : 0;

            int length = result.Length;
            int integerLength = (isInteger(result)) ? length : result.IndexOf(".");

            return (integerLength > maxIntegerLength);
        }
        
    }
}
