using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LinqTwit.QueryModule
{
    public interface ILoginController
    {
        string Username { get; set; }
        string Password { get; set; }
    }
}
