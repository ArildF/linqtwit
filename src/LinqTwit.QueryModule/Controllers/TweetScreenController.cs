using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

namespace LinqTwit.QueryModule.Controllers
{
    public class TweetScreenController : ITweetScreenController
    {
        private readonly IEventAggregator _aggregator;
        private readonly IRegion _region;
        private readonly ITweetScreenFactory _factory;
        private readonly RefreshEvent _refreshEvent;

        public TweetScreenController(IEventAggregator aggregator, IRegion region, ITweetScreenFactory factory)
        {
            _aggregator = aggregator;
            _region = region;
            _factory = factory;

            _aggregator.GetEvent<AuthorizationStateChangedEvent>().Subscribe(
                WhenAuthorizationStateChanged, true);

            _refreshEvent = _aggregator.GetEvent<RefreshEvent>();

        }

        private void WhenAuthorizationStateChanged(bool authorized)
        {
            if (authorized)
            {
                IQueryResultsViewModel model = _factory.Create("Default");
                _region.Add(model.View);
                _region.Activate(model.View);

                _refreshEvent.Publish(null);
            }
        }
    }
}
