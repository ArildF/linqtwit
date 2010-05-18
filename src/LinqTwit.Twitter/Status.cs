using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace LinqTwit.Twitter
{
    [XmlType("status")]
    public class Status
    {
        [XmlElement("id")]
        public long Id { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("created_at")]
        public string createdAt;

        public DateTime CreatedAt
        {
            get { return DateTime.ParseExact(this.createdAt, "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture); }
        }


        [XmlElement("user")]
        public User User { get; set; }

    }
}