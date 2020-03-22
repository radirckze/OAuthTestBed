using System;
using System.Collections.Generic;
using System.Text;

namespace GitLabOAuthClient
{
    public class GitLabUser
    {

        public int id { get; set; }

        public string name { get; set; }
        
        public string username { get; set; }

        public string state { get; set; }


        public string UserDetails()
        {
            return String.Format("ID = {0}, Name = {1}, Username = {2}, State = {3}", id, name, username, state);
        }
    }
}
