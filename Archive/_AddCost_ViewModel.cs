using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Archive
{
    class _AddCost_ViewModel
    {
        private string Validate(string result)
        {
            if (Notifications != null) Notifications.Clear();

            if (result != "0.")
            {
                double value = Convert.ToDouble(result, App.StandardRegionalInformation);

                if (value < 0)
                {
                    disableForm();
                    AddNotification(CreateNotification(ValidatorMessage.Negative));
                }
                else if (value > _upperLimit)
                {
                    disableForm();
                    AddNotification(CreateNotification(ValidatorMessage.MaxValue));
                }
                else if (result.Length == _calculator.MaxOutputLength-1)
                {
                    AddNotification(CreateNotification(ValidatorMessage.MaxOutputLength));
                    enebleForm();
                }
                else if (isMaxDecimal(result, _calculator.MaxDecimal))
                {
                    AddNotification(CreateNotification(ValidatorMessage.MaxDecimalLength));
                    enebleForm();
                }
                else
                {
                    enebleForm();
                }
            }

            return result;

        }
    }
}
