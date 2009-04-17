using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;
using LinqTwit.Utilities;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryResultsViewModel : IQueryResultsViewModel
    {
        private readonly IEventAggregator aggregator;
        private readonly ILinqApi api;

        public QueryResultsViewModel(IQueryResultsView view, IEventAggregator aggregator, ILinqApi api)
        {
            this.aggregator = aggregator;
            this.api = api;
            View = view;

            View.DataContext = this;

            this.Tweets = new ObservableCollection<TweetViewModel>();

            this.aggregator.GetEvent<QuerySubmittedEvent>().Subscribe(
                QuerySubmitted);
            this.aggregator.GetEvent<AuthorizationStateChangedEvent>().Subscribe
                (
                AuthorizationStateChanged);
        }

        private void AuthorizationStateChanged(bool newState)
        {
            if (newState)
            {
                var statuses = this.api.FriendsTimeLine();
                this.SetStatuses(statuses); 
            }
        }

        private void QuerySubmitted(string query)
        {
            var statuses = this.api.UserTimeLine(query);
            SetStatuses(statuses);
        }

        private void SetStatuses(IEnumerable<Status> statuses)
        {
            this.Tweets.Clear();
            statuses.Select(s => new TweetViewModel(s)).ForEach((this.Tweets.Add));
        }

        public IQueryResultsView View
        {
            get;
            private set;
        }

        public ObservableCollection<TweetViewModel> Tweets { get; private set; }
    }
}
