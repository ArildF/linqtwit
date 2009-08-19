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
                var mainView = CreateView(_timeLineFactory.CreateFriendsTimeLine(), "Main timeline" );
                CreateView(_timeLineFactory.CreateMentionsTimeLine(), "Mentions");

                _region.Activate(mainView);

                _refreshEvent.Publish(null);
            }
        }

        private IQueryResultsView CreateView(ITimeLineService timeLineService, string viewName)
        {
            IQueryResultsViewModel vm = _factory.Create(viewName,
                                                        timeLineService);

            _region.Add(vm.View);

            return vm.View;

        }
    }
}
