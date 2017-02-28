using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Common
{
    public static class SingleEquation
    {
        //Fields
        private static double _leftOperand = 0;
        private static double _rightOperand = double.NaN;
        private static string _operator = "=";
        private static double _result = double.NaN;
        private const string _digitFormat = "######################0.#########################";

        //Equation Properties
        public static double LeftOperand
        {
            get { return _leftOperand; }
            set { _leftOperand = value; }
        }
        public static double RightOperand
        {
            get { return _rightOperand; }
            set { _rightOperand = value; }
        }

        public static string Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        //Logical Properties
        private static bool AreOperandsProvided()
        {
            return (!LeftOperand.Equals(double.NaN) & !RightOperand.Equals(double.NaN));
        }
        private static bool AreOperatorsProvided()
        {
            return (!_operator.Equals(null) & !_operator.Equals("="));
        }
        public static bool IsExecutable()
        {
            return (AreOperandsProvided() & AreOperatorsProvided());
        }

        //Methods

        public static void Run(string actionButton)
        {
            if (IsExecutable())
            {
                switch (_operator)
                {
                    case "+":
                        Addition();
                        break;
                    case "-":
                        Subtraciton();
                        break;
                    case "*":
                        Multiplication();
                        break;
                    case "/":
                        Division();
                        break;
                    default:
                        break;
                }

                _operator = actionButton;
            }

        }
        public static void UpdateEquation(string value, string actionButton)
        {
            if (AreOperatorsProvided())
            {
                RightOperand = Convert.ToDouble(value, App.StandardRegionalInformation);
                Run(actionButton);
                LeftOperand = Convert.ToDouble(_result, App.StandardRegionalInformation);
            }
            else
            {
                LeftOperand = Convert.ToDouble(value, App.StandardRegionalInformation);
                Operator = actionButton;
                _result = LeftOperand;
            }
        }

        //Mathematical action

        private static void Addition()
        {
            _result = (_leftOperand + _rightOperand);
        }
        private static void Subtraciton()
        {
            _result = (_leftOperand - _rightOperand);
        }
        private static void Multiplication()
        {
            _result = (_leftOperand * _rightOperand);
        }
        private static void Division()
        {
            if (_rightOperand != 0)
            {
                _result = (_leftOperand / _rightOperand);
            }
        }

        public static string GetResult()
        {
            return _result.ToString(_digitFormat, App.StandardRegionalInformation);
        }

        public static string GetRoundedResult(int roundedDigits)
        {
            return Math.Round(_result, roundedDigits).ToString(_digitFormat, App.StandardRegionalInformation);
        }

    }
}
