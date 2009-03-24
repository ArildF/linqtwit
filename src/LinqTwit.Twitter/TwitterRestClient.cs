using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LinqTwit.Twitter
{
    class TwitterRestClient : ClientBase<ITwitterRestServiceContract>, ITwitterRestServiceContract
    {
        public Status GetStatus(string id)
        {
            return this.Channel.GetStatus(id);
        }
    }
}
