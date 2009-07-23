using System;
using LinqTwit.Twitter;

namespace LinqTwit.Linq.Impl
{
    class Tweet : ITweet
    {
        private readonly Status status;

        public Tweet(Status status)
        {
            this.status = status;
        }

        public long Id
        {
            get { return long.Parse(status.Id); }
        }
    }
}