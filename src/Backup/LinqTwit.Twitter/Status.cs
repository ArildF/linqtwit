using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace LinqTwit.Twitter
{
    [DataContract(Namespace="", Name = "status")]
    public class Status
    {
        [DataMember(Name = "id", Order=2)]
        public string Id { get; set; }

        [DataMember(Name="text", Order=3)]
        public string Text { get; set; }

        [DataMember(Name = "created_at", Order=1)] private string createdAt;

        public DateTime CreatedAt
        {
            get { return DateTime.ParseExact(this.createdAt, "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture); }
        }


        [DataMember(Name="user", Order=10)]
        public User User { get; set; }

    }
}