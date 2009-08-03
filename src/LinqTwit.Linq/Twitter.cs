using System.Linq;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class Twitter
    {
        public Twitter(ILinqApi linqApi)
        {
            Users = new TwitterQueryable<IUser>(linqApi);
            Tweets = new TwitterQueryable<Status>(linqApi);
        }
        public IQueryable<IUser> Users { get; private set; }

        public IQueryable<Status> Tweets { get; private set; }
    }
}
