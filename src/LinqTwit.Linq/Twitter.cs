using System.Linq;

namespace LinqTwit.Linq
{
    public class Twitter
    {
        public Twitter(ILinqApi linqApi)
        {
            Users = new TwitterQueryable<IUser>(linqApi);
            Tweets = new TwitterQueryable<ITweet>(linqApi);
        }
        public IQueryable<IUser> Users { get; private set; }

        public IQueryable<ITweet> Tweets { get; private set; }
    }
}
