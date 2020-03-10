using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GitHubOAuthClient
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("GitHub OAuth Client test started!");

            GitSession gitSession = new GitSession(null); // will fail runtime

            try
            {
                WebRequest webRequest = WebRequest.Create("http://www.contoso.com/PostAccepter.aspx");
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add("client_id", gitSession.ClientId);
                webRequest.Headers.Add("redirect_uri", gitSession.CallbackURL);
                webRequest.Headers.Add("response_type", "token");
                webRequest.Headers.Add("state", gitSession.ClientSecret);

                WebResponse response = webRequest.GetResponse();

            }
            catch (WebException wex)
            {

            }
            catch (Exception ex)
            {

            }

            Console.WriteLine("someitng");
            Console.ReadLine();




        
        }
    }
}
