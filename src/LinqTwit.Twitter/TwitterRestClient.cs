using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;

namespace LinqTwit.Twitter
{
    public class TwitterRestClient : ClientBase<ITwitterRestServiceContract>, ITwitterRestServiceContract, ILinqApi
    {
        public TwitterRestClient(string endpointName) : base(endpointName)
        {
            
        }
        public Status GetStatus(string id)
        {
            return this.Channel.GetStatus(id);
        }

        Status[] ILinqApi.UserTimeLine(string user)
        {
            return UserTimeLine(user).ToArray();
        }

        Status[] ILinqApi.FriendsTimeLine()
        {
            return FriendsTimeLine().ToArray();
        }

        public void SetCredentials(string user, string pass)
        {
            this.ClientCredentials.UserName.UserName = user;
            this.ClientCredentials.UserName.Password = pass;
        }


        public Statuses FriendsTimeLine()
        {
            return Invoke(() => this.Channel.FriendsTimeLine());
        }

        private static T Invoke<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (MessageSecurityException exception)
            {
                throw new TwitterAuthorizationException("Failed to authorize against Twitter.", exception);                
            }

        }

        public Statuses UserTimeLine(string user)
        {
            return this.Channel.UserTimeLine(user);
        }
    }
}
