using System.Linq;

namespace LinqTwit.Linq
{
    public class Twitter
    {
        public Twitter()
        {
            Users = new TwitterQueryable<IUser>();
            Tweets = new TwitterQueryable<ITweet>();
        }
        public IQueryable<IUser> Users { get; private set; }

        public IQueryable<ITweet> Tweets { get; private set; }
    }
}
