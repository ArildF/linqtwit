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

            this.Statuses = new ObservableCollection<Status>();

            this.aggregator.GetEvent<QuerySubmittedEvent>().Subscribe(
                QuerySubmitted);
        }

        private void QuerySubmitted(string query)
        {
            var statuses = this.api.UserTimeLine(query);
            this.Statuses.Clear();
            statuses.ForEach(this.Statuses.Add);

        }

        public IQueryResultsView View
        {
            get;
            private set;
        }

        public ObservableCollection<Status> Statuses { get; private set; }
    }
}
