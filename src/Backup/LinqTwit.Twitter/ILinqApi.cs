namespace LinqTwit.Twitter
{
    public interface ILinqApi
    {
        Status GetStatus(string id);
        Status[] UserTimeLine(string user);
        Status[] FriendsTimeLine();


        void SetCredentials(string user, string pass);
    }
}