using System;
using System.Runtime.Serialization;

namespace LinqTwit.Twitter
{
    [DataContract(Namespace = "", Name = "user")]
    public class User
    {
        [DataMember(Name = "id", Order = 1)]
        public int Id { get; set; }

        [DataMember(Name = "screen_name", Order=3)]
        public string ScreenName { get; set; }

        [DataMember(Name = "name", Order=2)]
        public string Name { get; set; }


        [DataMember(Name = "location", Order=4)]
        public string Location { get; set; }

        [DataMember(Name = "description", Order=5)]
        public string Description { get; set; }


        [DataMember(Name = "profile_image_url", Order=6)]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name="followers_count", Order=9)]
        public int FollowersCount { get; set; }

    }
}