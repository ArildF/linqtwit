using LinqTwit.Core;

namespace LinqTwit.QueryModule
{
    public interface ITweetScreenFactory
    {
        IQueryResultsViewModel Create(string title, ITimeLineService service);

    }
}