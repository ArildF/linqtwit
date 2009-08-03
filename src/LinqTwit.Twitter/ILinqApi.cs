namespace LinqTwit.Twitter
{
    public interface ILinqApi
    {
        Status GetStatus(string id);
        Status[] UserTimeLine(string user);
        Status[] FriendsTimeLine(TimeLineArgs args);


        void SetCredentials(string user, string pass);
        Status Update(string status);
    }
}