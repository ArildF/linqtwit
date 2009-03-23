using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Linq
{
    public class Twitter
    {
        public Twitter()
        {
            Users = new TwitterQueryable<IUser>();

        }
        public IQueryable<IUser> Users { get; private set; }
    }
}
