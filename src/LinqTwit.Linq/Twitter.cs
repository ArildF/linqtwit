using System;
using System.Linq;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class Twitter : ITwitter
    {
        public Twitter(ILinqApi linqApi)
        {
            FriendsTimeLine = new TwitterQueryable<Status>(new TwitterQueryProvider(() => new TimelineQuery(linqApi.FriendsTimeLine)));
            MentionsTimeLine =
                new TwitterQueryable<Status>(
                    new TwitterQueryProvider(
                        () => new TimelineQuery(linqApi.MentionsTimeLine)));

            Users =
                new TwitterQueryable<IUser>(
                    new TwitterQueryProvider(() => new TimelineQuery(linqApi.FriendsTimeLine)));

        }
        public IQueryable<IUser> Users { get; private set; }

        public IQueryable<Status> FriendsTimeLine { get; private set; }
        public IQueryable<Status> MentionsTimeLine
        {
            get; private set;
        }
    }
}
