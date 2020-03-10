using System;
using System.Collections.Generic;
using System.Text;

using OAuthClientBase;

namespace GitHubOAuthClient
{
    public class GitSession : SessionBase
    {

        public GitSession(string initFile) : base(initFile)
        {

        }
    }
}
