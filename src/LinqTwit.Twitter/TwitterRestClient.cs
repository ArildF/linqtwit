using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LinqTwit.Twitter
{
    public class TwitterRestClient : ClientBase<ITwitterRestServiceContract>, ITwitterRestServiceContract, ILinqApi
    {
        public Status GetStatus(string id)
        {
            return this.Channel.GetStatus(id);
        }

        Status[] ILinqApi.UserTimeLine(string user)
        {
            return UserTimeLine(user).ToArray();
        }

        public Statuses UserTimeLine(string user)
        {
            return this.Channel.UserTimeLine(user);
        }
    }
}
