namespace LinqTwit.Core
{
    public interface ITimeLineFactory
    {
        ITimeLineService CreateFriendsTimeLine();
        ITimeLineService CreateMentionsTimeLine();
    }
}