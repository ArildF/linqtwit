using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using DevDefined.OAuth.Consumer;
using LinqTwit.Twitter.OAuth;

namespace LinqTwit.Twitter
{
    public class TwitterRestClient : ClientBase<ITwitterRestServiceContract>, ITwitterRestServiceContract, ILinqApi
    {
        public TwitterRestClient(string endpointName, IOAuthSession session) : base(endpointName)
        {
            Endpoint.Behaviors.Add(new OAuthBehavior(session));
        }

        public Status GetStatus(string id)
        {
            return this.Channel.GetStatus(id);
        }

        Status[] ILinqApi.UserTimeLine(string user, TimeLineArgs args)
        {
            return UserTimeLine(user, args).ToArray();
        }

        Status[] ILinqApi.FriendsTimeLine(TimeLineArgs args)
        {
            return ((TwitterRestClient) this).FriendsTimeLine(args).ToArray();
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

        public Statuses UserTimeLine(string user, string sinceId, string count, string maxId, string page)
        {
            return Invoke(() => this.Channel.UserTimeLine(user, sinceId, count, maxId, page));
        }

        public Statuses MentionsTimeLine(string sinceId, string count, string maxId, string page)
        {
            return Invoke(() => this.Channel.MentionsTimeLine(sinceId, count, maxId, page));
        }


        public Statuses FriendsTimeLine()
        {
            return Invoke(() => this.Channel.FriendsTimeLine(null, null, null, null));
        }

        public Statuses FriendsTimeLine(TimeLineArgs args)
        {
            return Invoke(() => CallFromArgs(args, this.Channel.FriendsTimeLine));
        }

        private static Statuses CallFromArgs(TimeLineArgs args, Func<string, string, string, string, Statuses> func)
        {
            return func(args.SinceId != null ? args.SinceId.ToString() : null,
                        args.Count != null ? args.Count.ToString() : null,
                        args.MaxId != null ? args.MaxId.ToString() : null,
                        args.Page != null ? args.Page.ToString() : null);
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
            return this.UserTimeLine(user, new TimeLineArgs());
        }

        public Statuses UserTimeLine(string user, TimeLineArgs args)
        {
            return Invoke(() => this.Channel.UserTimeLine(
                                    user,
                                    args.SinceId != null
                                        ? args.SinceId.ToString()
                                        : null,
                                    args.Count != null
                                        ? args.Count.ToString()
                                        : null,
                                    args.MaxId != null
                                        ? args.MaxId.ToString()
                                        : null,
                                    args.Page != null
                                        ? args.Page.ToString()
                                        : null));
        }

        public Status[] MentionsTimeLine(TimeLineArgs args)
        {
            return Invoke(() => CallFromArgs(args, MentionsTimeLine)).ToArray();
        }
    }
}
