using System.Runtime.Serialization;

namespace LinqTwit.Twitter
{
    [DataContract(Namespace = "", Name = "user")]
    public class User
    {
        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}