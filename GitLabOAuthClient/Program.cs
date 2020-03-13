using System;
using System.Net.Http;
using System.Threading.Tasks;

using OAuthClientBase;

namespace GitLabOAuthClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string GitLabAuthUrl = "https://gitlab.helpsystems.com/oauth/token";

            Console.WriteLine("GitLab Auth application");

            Console.WriteLine("Testing password credential flow");
            Console.Write("Username:");
            string username = Utilities.MaskedRead();
            Console.WriteLine();
            Console.Write("Password:");
            string password = Utilities.MaskedRead();

            HttpResponseMessage response = Utilities.PWCredentialFlow(GitLabAuthUrl, username, password).Result;


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //get heading

            }


        }
    }
}
