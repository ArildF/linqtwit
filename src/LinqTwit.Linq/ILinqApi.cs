using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public interface ILinqApi
    {
        Status GetStatus(string id);
    }
}