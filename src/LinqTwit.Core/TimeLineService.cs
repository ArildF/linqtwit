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

        public IEnumerable<Status> GetOlder(Status status)
        {
            long olderThanId = status.Id;
            var statuses = (from s in _queryable where s.Id <= olderThanId select s).ToArray();
            if (statuses.First().Id == status.Id)
            {
                return statuses.Skip(1);
            }

            return statuses;
        }

        public IEnumerable<Status> GetNewer(Status status)
        {
            long newerThanId = status.Id;
            var statuses =
                (from s in _queryable where s.Id >= newerThanId select s).
                    ToArray();

            if (statuses.Any() && statuses.Last().Id == status.Id)
            {
                return statuses.Take(statuses.Count() - 1);
            }

            return statuses;
        }
    }
}
