﻿using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Web;

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

        Status[] ILinqApi.FriendsTimeLine(FriendsTimeLineArgs args)
        {
            return FriendsTimeLine().ToArray();
        }

        public void SetCredentials(string user, string pass)
        {
            this.ClientCredentials.UserName.UserName = user;
            this.ClientCredentials.UserName.Password = pass;
        }

        public Status Update(string status)
        {
            return this.Channel.Update(status);
        }

        public Statuses FriendsTimeLine(string sinceId, string count, string maxId, string page)
        {
            return Invoke(() => this.Channel.FriendsTimeLine(sinceId, count, maxId, page));
        }


        public Statuses FriendsTimeLine()
        {
            return Invoke(() => this.Channel.FriendsTimeLine(null, null, null, null));
        }

        public Statuses FriendsTimeLine(FriendsTimeLineArgs args)
        {
            return Invoke(() => this.Channel.FriendsTimeLine(
                args.SinceId != null ? args.SinceId.ToString() : null,
                args.Count != null ? args.Count.ToString() : null,
                args.MaxId != null ? args.MaxId.ToString() : null,
                args.Page != null ? args.Page.ToString() : null));
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
