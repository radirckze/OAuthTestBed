using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using OAuthClientBase;

namespace GitLabOAuthClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string GitLabAuthUrl = "https://gitlab.helpsystems.com/oauth/token";
            string GitLabUserUrl = "https://gitlab.helpsystems.com/api/v4/user";

            Console.WriteLine("GitLab Auth application");

            Console.WriteLine("Testing password credential flow");
            Console.Write("Username:");
            string username = Utilities.MaskedRead();
            Console.WriteLine();
            Console.Write("Password:");
            string password = Utilities.MaskedRead();
            Console.WriteLine();

            HttpResponseMessage response = Utilities.PWCredentialFlow(GitLabAuthUrl, username, password).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                PasswordCFContent passwordCFContent = null;
                try
                {
                    HttpContent content = response.Content;
                    string strContent = content.ReadAsStringAsync().Result;
                    passwordCFContent = JsonSerializer.Deserialize<PasswordCFContent>(strContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occurred during initial authorization .... Program exiting.");
                    Environment.Exit(-1);
                    
                }

                if (passwordCFContent !=null)
                {
                    //lets make a call to GitLab and validate the token works.
                    HttpClient authenticatedClient = ConstructAuthenticatedClient(passwordCFContent);

                    try
                    {
                        response = authenticatedClient.GetAsync(GitLabUserUrl).Result;
                        HttpContent content = response.Content;
                        string strContent = content.ReadAsStringAsync().Result;
                        GitLabUser user = JsonSerializer.Deserialize<GitLabUser>(strContent);
                        Console.WriteLine("User data received from GitLab : {0}", user.UserDetails());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception occurred during data retrieval .... Program exiting.");
                    }

                }
                
            }

            Console.WriteLine("Testing password credential flow done!");
        }

        private static HttpClient ConstructAuthenticatedClient(PasswordCFContent passwordCFContent)
        {
            HttpClient authenticatedClient = new HttpClient();
            authenticatedClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", passwordCFContent.access_token));
            //authenticatedClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            return authenticatedClient;

        }
    }
}
