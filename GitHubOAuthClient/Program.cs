using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using OAuthClientBase;

namespace GitHubOAuthClient
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("GitHub OAuth Client test started!");

            bool accessGranted = false;
            string access_code = null;

            // read configuration 
            string client_id = ConfigurationManager.AppSettings.Get("client_id");
            string client_secret = ConfigurationManager.AppSettings.Get("client_secret");
            string redirectUrl = ConfigurationManager.AppSettings.Get("redirectUrl");
            string username = ConfigurationManager.AppSettings.Get("username");

            // Setup HTTP listner to receive access key, if user grants permission. 
            Task<HttpListenerRequest> httpListnerTask = SimpleHttpListenerAsync(redirectUrl);

            //construct GitHub account access request url. Launch browser with url
            string ghAccessRequestUrl = ConstructGHRequestUrl(client_id, redirectUrl, username);
            Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "\"C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe\"";
            myProcess.StartInfo.ArgumentList.Add(ghAccessRequestUrl);
            myProcess.Start();

            HttpListenerRequest request = httpListnerTask.Result; //Note, this is blocking. Also, its OK to block in console apps - no risk of deadlocks.

            //Get the temporary code from the request ...
            string tempCode = GetTempCodeFromRequest(request);

            if (tempCode != null)
            {
                //Exchange temp code for access token.

                // Setup HTTP listner to receive the access code.
                // For GitHub at least this is not necessary. The token is returned in the response to the request. Need to check the spec for expected bahavior
                //httpListnerTask = SimpleHttpListenerAsync(redirectUrl);  

                //Construct request for permenent code
                //string ghAccessCodeRequestUrl = ConstructAuthCodeRequestUrl(client_id, client_secret, tempCode, redirectUrl);
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("client_id", client_id);
                parameters.Add("client_secret", client_secret);
                parameters.Add("code", tempCode);
                parameters.Add("redirect_uri", redirectUrl);

                

                //Send request 
                Task<HttpResponseMessage> userAccessTask = Utilities.HttpRequestWrapper("https://github.com/login/oauth/access_token", HttpMethod.Post, ParamOption.Params, parameters); //*** UPDATE this - send params

                HttpResponseMessage response = userAccessTask.Result; //block waiting for permenent code
                access_code = GetAccessCodeFromResponseBody(response);
                PrintHttpResponseMessageDetails(response);

                //request = httpListnerTask.Result;
                //access_code = //get access_code from request.

                if (access_code != null && access_code.Length > 0)
                {
                    Console.WriteLine("You have been granted access!");
                    accessGranted = true;
                }
                else
                {
                    Console.WriteLine("Failed to exchnage temporary code for access token.");
                }

                // use code to access account ...
            }

            if (accessGranted)
            {
                // Lets access the site to confirm ...
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("Authorization", string.Format("token {0}", access_code));
                parameters.Add("User-Agent", "test");
                Task<HttpResponseMessage> userAccessTask = Utilities.HttpRequestWrapper("https://api.github.com/user", HttpMethod.Get, ParamOption.Headers, parameters);
                HttpResponseMessage response = userAccessTask.Result;

                string userContent = response.Content.ReadAsStringAsync().Result;

                Console.WriteLine(String.Format("The user content is: \n{0}", userContent == null ? "null" : userContent));
                Console.WriteLine();
            }



            //System.Collections.Specialized.NameValueCollection headers = request.Headers;
            //foreach(string key in headers.AllKeys)
            //{
            //    Console.WriteLine(String.Format("Key={0}, Value={1}", key, headers[key]));
            //}
            //string requestPostData = GetRequestPostData(request);
            //Console.WriteLine(String.Format("Request post data: {0}", requestPostData == null ? "null" : requestPostData));





            //GitSession gitSession = new GitSession(null); // will fail runtime

            //try
            //{
            //    WebRequest webRequest = WebRequest.Create("http://www.contoso.com/PostAccepter.aspx");
            //    webRequest.Method = "GET";
            //    webRequest.ContentType = "application/json";
            //    webRequest.Headers.Add("client_id", gitSession.ClientId);
            //    webRequest.Headers.Add("redirect_uri", gitSession.CallbackURL);
            //    webRequest.Headers.Add("response_type", "token");
            //    webRequest.Headers.Add("state", gitSession.ClientSecret);

            //    WebResponse response = webRequest.GetResponse();

            //}
            //catch (WebException wex)
            //{

            //}
            //catch (Exception ex)
            //{

            //}

            Console.WriteLine("Done");
            Console.ReadLine();
        
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=netframework-4.8
        public static void SimpleHttpListener(string listnerUrl = null)
        {

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Http Listner not supported on this machine.");
                return;
            }
            
            if (listnerUrl == null)
            {
                listnerUrl = "http://localhost:8088/";
            }
                
            // Create a listener.
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listnerUrl);
            listener.Start();
            Console.WriteLine("Listening on {0} ...", listnerUrl);
             
            // Wait for request.
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            //Send reponse
            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            // Listener's job is done.
            listener.Stop();
            Console.WriteLine("Listener stopped");
        }

        // Async version so thread can return while waiting for Gitlab server to send the 
        public static async Task<HttpListenerRequest> SimpleHttpListenerAsync(string listnerUrl = null)
        {

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Http Listner not supported on this machine.");
                return null;
            }

            if (listnerUrl == null)
            {
                listnerUrl = "http://localhost:8088/";
            }

            // Create a listener 
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listnerUrl);
            listener.Start();
            Console.WriteLine("Listening on {0} ...", listnerUrl);

            // Wait for request.
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;

            //Send reponse
            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY>OK</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            // Listener's job is done.
            listener.Stop();
            Console.WriteLine("Listener stopped");
            return request;
        }

        private static string ConstructGHRequestUrl(string client_id, string redirectUrl, string username)
        {
            return String.Format("https://github.com/login/oauth/authorize?client_id={0}&redirect_uri={1}&login={2}&scope=repo", client_id, redirectUrl, username);
        }

        private static string ConstructAuthCodeRequestUrl(string client_id, string client_secret, string code, string redirectUrl)
        {
            return String.Format("https://github.com/login/oauth/access_token?client_id={0}&client_secret={1}&code={2}&redirect_uri={3}", 
                client_id, client_secret, code, redirectUrl);
        }

        // Get post data from ListnerRequest, if any.
        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string GetTempCodeFromRequest(HttpListenerRequest request)
        {
            string tempCode = null;
            System.Collections.Specialized.NameValueCollection queryStrings = request.QueryString;
            if (queryStrings != null && queryStrings.Count > 0)
            {
                foreach (string key in queryStrings.AllKeys)
                {
                    //Console.WriteLine(String.Format("Query string Key={0}, Value={1}", key, queryStrings[key]));
                    if (key.Equals("code"))
                    {
                        tempCode = queryStrings[key];
                        break;
                    }
                }
            }
            return tempCode;
        }

        public static string GetAccessCodeFromResponseBody(HttpResponseMessage response)
        {
            //sample content: access_token=c482858adb39d27e2ea8810fe328de8716ae8137&scope=repo&token_type=bearer
            string accessCode = null;
            string messageContent = response.Content.ReadAsStringAsync().Result;
            string[] parts = messageContent.Split('&');
            if (parts != null && parts.Length>0)
            {
                foreach(string part in parts)
                {
                    if (part.Contains("access_token"))
                    {
                        accessCode= part.Substring(part.IndexOf('=') + 1);
                        break;
                    }
                }
            }
            return accessCode;
        }

        private static void PrintHttpResponseMessageDetails(HttpResponseMessage response)
        {

            HttpResponseHeaders headers = response.Headers;
            IEnumerator<KeyValuePair<string, IEnumerable<string>>> enumerator = headers.GetEnumerator();

            Console.WriteLine("HttpResponseMessage details ...");
            Console.WriteLine("Headers:");
            do
            {
                KeyValuePair<string, IEnumerable<string>> currentHeader = enumerator.Current;
                Console.WriteLine(String.Format("\tKey= {0}, Value= {1}", currentHeader.Key, currentHeader.Value));
            }
            while (enumerator.MoveNext()) ;

            string messageContent = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("Content:");
            Console.WriteLine(String.Format("\t{0}", messageContent));
            Console.WriteLine();


                
         }

    }
}
