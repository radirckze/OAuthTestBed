﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GitLabOAuthClient
{
    //Password credential flow contet
    public class PasswordCFContent
    {

        public string access_token { get; set; }

        public string refresh_token { get; set; }
        
        public string token_type { get; set; }

        public string expires_in { get; set; }


    }
}
