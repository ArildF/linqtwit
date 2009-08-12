using LinqTwit.Core;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

namespace LinqTwit.QueryModule.Controllers
{
    public class TweetScreenController : ITweetScreenController
    {
        private readonly IEventAggregator _aggregator;
        private readonly IRegion _region;
        private readonly ITweetScreenFactory _factory;
        private readonly ITimeLineFactory _timeLineFactory;
        private readonly RefreshEvent _refreshEvent;

        public TweetScreenController(IEventAggregator aggregator, IRegion region, 
            ITweetScreenFactory screenFactory, ITimeLineFactory timeLineFactory)
        {
            _aggregator = aggregator;
            _region = region;
            _factory = screenFactory;
            _timeLineFactory = timeLineFactory;

            _aggregator.GetEvent<AuthorizationStateChangedEvent>().Subscribe(
                WhenAuthorizationStateChanged, true);

            _refreshEvent = _aggregator.GetEvent<RefreshEvent>();

        }

        private void WhenAuthorizationStateChanged(bool authorized)
        {
            if (authorized)
            {
                ITimeLineService timeLineService =
                    _timeLineFactory.CreateFriendsTimeLine();

                IQueryResultsViewModel model = _factory.Create("Main timeline", timeLineService);
                _region.Add(model.View);
                _region.Activate(model.View);

                _refreshEvent.Publish(null);
            }
        }
    }
}
