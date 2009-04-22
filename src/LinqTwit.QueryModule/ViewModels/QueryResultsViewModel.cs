using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using LinqTwit.Infrastructure;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryResultsViewModel : IQueryResultsViewModel, INotifyPropertyChanged, IRaisePropertyChanged
    {
        private readonly IEventAggregator aggregator;
        private readonly ILinqApi api;
        private TweetViewModel selectedTweet;

        public event PropertyChangedEventHandler PropertyChanged;

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

            GlobalCommands.UpCommand.RegisterCommand(new DelegateCommand<object>(MoveUp));
            GlobalCommands.DownCommand.RegisterCommand(new DelegateCommand<object>(MoveDown));
        }

        private void MoveDown(object obj)
        {
            this.SelectedIndex = this.SelectedIndex < (this.Tweets.Count - 1)
                                     ? this.SelectedIndex + 1
                                     : this.SelectedIndex;
        }

        private void MoveUp(object obj)
        {
            this.SelectedIndex = this.SelectedIndex > 0
                                     ? (this.SelectedIndex - 1)
                                     : 0;

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
            this.SelectedTweet = this.Tweets.Count > 0 ? this.Tweets[0] : null;
            SelectedIndex = this.Tweets.Count > 0 ? 0 : -1;
        }

        private int SelectedIndex
        {
            get
            {
                return this.Tweets.IndexOf(this.SelectedTweet);
            }
            set
            {
                this.SelectedTweet = value >= 0
                                         ? this.Tweets[value]
                                         : null;
            }
        }

        public IQueryResultsView View
        {
            get;
            private set;
        }

        public ObservableCollection<TweetViewModel> Tweets { get; private set; }

        public TweetViewModel SelectedTweet
        {
            get
            {
                return this.selectedTweet;
            }
            set
            {
                if (selectedTweet != value)
                {
                    selectedTweet = value;
                    this.OnPropertyChanged(p => p.SelectedTweet);
                }
            }
        }

        void IRaisePropertyChanged.RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
