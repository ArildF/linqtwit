namespace LinqTwit.Twitter
{
    public interface ILinqApi
    {
        Status GetStatus(string id);
        Status[] UserTimeLine(string user);
        Status[] FriendsTimeLine(FriendsTimeLineArgs args);


        void SetCredentials(string user, string pass);
        Status Update(string status);
    }
}