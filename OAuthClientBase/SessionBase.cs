using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthClientBase
{
    public class SessionBase
    {

        public string BaseUrl { get; set; }

        public string AuthEndpoint { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CallbackURL { get; set; }

        public string AccessToken { get; set; }

        public string Scope { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }


        public SessionBase(string initFile)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

      
}
