using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Twitter
{
    public class TimeLineArgs
    {
        public long? SinceId { get; set; }

        public int? Count { get; set; }

        public long? MaxId { get; set; }

        public int? Page { get; set; }
    }
}
