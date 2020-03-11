using System;

using OAuthClientBase;

namespace GitLabOAuthClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Type any text and <enter> to terminate:");
            string inputStr = string.Empty;
            do
            {
                inputStr = Utilities.MaskedRead();
                Console.WriteLine(String.Format("The input string is {0}", inputStr));
            }
            while (inputStr != null && inputStr.Length > 1);


        }
    }
}
