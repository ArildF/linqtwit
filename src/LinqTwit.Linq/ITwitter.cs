using System.Linq;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public interface ITwitter
    {
        IQueryable<IUser> Users { get; }
        IQueryable<Status> FriendsTimeLine { get; }
    }
}