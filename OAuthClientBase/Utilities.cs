using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public static async Task<HttpResponseMessage> PWCredentialFlow(string url, string username, string password)
        {
            try
            {
                
                using (HttpClient client = new HttpClient())
                {
                    string pwcUrl = ConstructPWCredentialUrl(url, username, password);
                    HttpResponseMessage response = await client.PostAsync(url, null);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string ConstructPWCredentialUrl(string url, string username, string password)
        {
            return (String.Format("{0}?grant_type=password&username={1}&password={2}", url, username, password));
        }
    }
}
