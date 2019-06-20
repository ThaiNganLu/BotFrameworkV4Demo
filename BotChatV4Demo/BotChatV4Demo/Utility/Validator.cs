using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class Validator
    {
        public static bool ValidateQuantity(string input, out int quantity, out string message)
        {
            int maxQuantityOrder = 10;
            quantity = 0;
            message = null;

            try
            {
                if (int.Parse(input) > 0 && int.Parse(input) <= maxQuantityOrder)
                    return true;

                message = "I'm sorry, 10 products available now. Please enter a quantity between 1 and 10.";
                return false;
            }
            catch
            {
                message = "I'm sorry, I didn't understand it. Please enter a quantity between 1 and 10.";
                return false;
            }
        }
    }
}
