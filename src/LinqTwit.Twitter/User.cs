using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace LinqTwit.Twitter
{
    [XmlType("user")]
    public class User
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("screen_name")]
        public string ScreenName { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }


        [XmlElement("location")]
        public string Location { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }


        [XmlElement("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [XmlElement("followers_count")]
        public int FollowersCount { get; set; }

    }
}