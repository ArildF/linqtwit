using System.Collections.Generic;
using LinqTwit.Twitter;

namespace LinqTwit.Core
{
    public interface ITimeLineService
    {
        IEnumerable<Status> GetLatest();
    }
}