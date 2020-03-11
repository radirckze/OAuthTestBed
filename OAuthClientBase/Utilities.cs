using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthClientBase
{
    public class Utilities
    {

        public static string MaskedRead()
        {
            StringBuilder inputStr = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (inputStr.Length > 0)
                    {
                        inputStr.Remove(inputStr.Length-1, 1);
                        Console.Write("\b \b");
                    }
                }
                else 
                {
                    inputStr.Append(i.KeyChar); //not checking for invalid characters
                    Console.Write("*");
                }
            }
            return inputStr.ToString(); ;
        }
    }
}
