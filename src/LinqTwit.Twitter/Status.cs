using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace LinqTwit.Twitter
{
    [DataContract(Namespace="", Name = "status")]
    public class Status
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name="text")]
        public string Text { get; set; }

        [DataMember(Name = "created_at")] private string createdAt;

        public DateTime CreatedAt
        {
            get { return DateTime.ParseExact(this.createdAt, "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture); }
        }


        [DataMember(Name="user")]
        public User User { get; set; }
    }
}