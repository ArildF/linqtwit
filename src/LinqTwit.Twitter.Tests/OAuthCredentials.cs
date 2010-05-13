using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Framework;

namespace LinqTwit.Twitter.Tests
{
    [Serializable]
    class OAuthCredentials
    {
        public IToken AccessToken { get; set; }
        public string ConsumerSecret { get; set; }
        public string ConsumerKey { get; set; }
    }
}
