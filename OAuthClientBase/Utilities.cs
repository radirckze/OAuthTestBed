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
                    HttpResponseMessage response = await client.PostAsync(pwcUrl, null);
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

        private static HttpRequestMessage CreateRequestMessage(string url, ParamOption paramOption, Dictionary<string, string> parameters=null, string content = null)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri(url);
         
            // Add headers or parameters per paramOption
            if (paramOption == ParamOption.Headers)
            {
                foreach (string key in parameters.Keys)
                {
                    requestMessage.Headers.Add(key, parameters[key]);
                }
            }
            else if (paramOption == ParamOption.Params)
            {
                string tempUrl = url;
                if (parameters != null && parameters.Count > 0)
                {
                    if (tempUrl.EndsWith("/"))
                    {
                        tempUrl = tempUrl.Remove(tempUrl.Length - 2);
                    }
                    tempUrl = tempUrl + "?";
                    foreach (string key in parameters.Keys)
                    {
                        tempUrl = tempUrl + String.Format("{0}={1}&", key, parameters[key]);
                    }
                    tempUrl = tempUrl.Remove(tempUrl.Length - 2);
                }
                requestMessage.RequestUri = new Uri(tempUrl);
            }

            if (content != null && content.Length > 0)
            {
                requestMessage.Content = new StringContent(content);
            }

            return requestMessage;
        }

        public static async Task<HttpResponseMessage> HttpRequestWrapper(string url, HttpMethod methodType, ParamOption paramOption, 
            Dictionary<string, string> parameters = null, string content = null)
        {
            try
            {
                HttpRequestMessage request = CreateRequestMessage(url, paramOption, parameters, content);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.SendAsync(request) ;
                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
