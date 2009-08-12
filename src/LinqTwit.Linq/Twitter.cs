using System.Linq;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class Twitter : ITwitter
    {
        public Twitter(ILinqApi linqApi)
        {
            FriendsTimeLine = new TwitterQueryable<Status>(new TwitterQueryProvider(() => new TimelineQuery(linqApi)));
            Users =
                new TwitterQueryable<IUser>(
                    new TwitterQueryProvider(() => new TimelineQuery(linqApi)));

        }
        public IQueryable<IUser> Users { get; private set; }

        public IQueryable<Status> FriendsTimeLine { get; private set; }
    }
}
