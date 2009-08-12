using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Linq;
using LinqTwit.Twitter;

namespace LinqTwit.Core
{
    public class TimeLineService : ITimeLineService
    {
        private readonly IQueryable<Status> _queryable;

        public TimeLineService(IQueryable<Status> queryable)
        {
            _queryable = queryable;
        }

        public IEnumerable<Status> GetLatest()
        {
            return (from status in _queryable select status).ToArray();
        }
    }
}
