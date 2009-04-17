using Microsoft.Practices.Composite;

namespace LinqTwit.QueryModule
{
    public interface IQueryEntryView : IActiveAware
    {
        void SetModel(IQueryEntryViewModel model);

    }
}
