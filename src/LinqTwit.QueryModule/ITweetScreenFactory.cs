namespace LinqTwit.QueryModule
{
    public interface ITweetScreenFactory
    {
        IQueryResultsViewModel Create(string title);

    }
}