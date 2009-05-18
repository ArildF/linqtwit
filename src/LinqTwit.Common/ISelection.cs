using LinqTwit.Twitter;

namespace LinqTwit.Common
{
    public interface ISelection
    {
        Status SelectedTweet { get; }
    }
}