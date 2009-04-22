using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure.Properties;

namespace LinqTwit.Infrastructure.ApplicationServices
{
    public class CredentialsStore : ICredentialsStore
    {
        public string Password
        {
            get { return Settings.Default.Password; }
            set{ Settings.Default.Password = value;}
        }
        

        public string Username
        {
            get { return Settings.Default.Username; }
            set{ Settings.Default.Username = value;}
        }

        public void PersistCredentials()
        {
            Settings.Default.Save();
        }
    }
}
