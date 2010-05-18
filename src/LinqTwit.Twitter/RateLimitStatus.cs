using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace LinqTwit.Twitter
{
    [XmlType("hash")]
    public class RateLimitStatus
    {
        [XmlElement("remaining-hits")]
        public int RemainingHits { get; set; }

        [XmlElement("hourly-limit")]
        public int HourlyLimit { get; set; }

        [XmlElement("reset-time-in-seconds")]
        public int ResetTimeInSeconds{ get; set;}
    }
}
