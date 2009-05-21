using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Infrastructure
{
    public interface ICredentialsStore
    {
        string Password { get; set; }
        string Username { get; set; }
        void PersistCredentials();
    }
}
