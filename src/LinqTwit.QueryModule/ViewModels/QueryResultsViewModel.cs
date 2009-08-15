using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using LinqTwit.Common;
using LinqTwit.Core;
using LinqTwit.Infrastructure;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryResultsViewModel : ViewModelBase, IQueryResultsViewModel
    {
        private readonly IEventAggregator _aggregator;
        private readonly ITimeLineService _service;
        private TweetViewModel selectedTweet;
        private readonly IAsyncManager asyncManager;
        private IList<MenuViewModel> _contextMenu;
        private ICommand _editCommand;
        private ICommand _cancelEditCommand;

        public QueryResultsViewModel(string caption, IQueryResultsView view, IEventAggregator aggregator, ITimeLineService service, 
            IAsyncManager asyncManager, ContextMenuRoot menu)
        {
            this._aggregator = aggregator;
            _service = service;
            this.asyncManager = asyncManager;
            Caption = caption;
            View = view;

            View.DataContext = this;

            this.Tweets = new ObservableCollection<TweetViewModel>();

            this._aggregator.GetEvent<RefreshEvent>().Subscribe(Refresh//,  
                ,ThreadOption.UIThread, true,
                _ => !this.Editing
                );

            GlobalCommands.UpCommand.RegisterCommand(new DelegateCommand<object>(MoveUp));
            GlobalCommands.DownCommand.RegisterCommand(new DelegateCommand<object>(MoveDown));

            _contextMenu = menu;

            _editCommand = new DelegateCommand<object>(EditSelectedTweet,
                                                       o =>
                                                       this.SelectedTweet !=
                                                       null);

            _cancelEditCommand = new DelegateCommand<object>(CancelEdit,
                o => this.SelectedTweet != null && this.SelectedTweet.Editable);
        }

        protected bool Editing
        {
            get {return this.SelectedTweet != null && this.SelectedTweet.Editable; }
        }

        private void CancelEdit(object obj)
        {
            if (this.SelectedTweet != null && this.SelectedTweet.Editable)
            {
                this.SelectedTweet.Editable = false;
            }
        }

        private void EditSelectedTweet(object obj)
        {
            if (this.SelectedTweet != null)
            {
                this.SelectedTweet.Editable = true;
            }
        }

        public IList<MenuViewModel> ContextMenu
        {
            get
            {
                return _contextMenu;
            }
        }

        private void Refresh(object _)
        {
            this.GetFriendsTimeLine();
        }

        private void MoveDown(object obj)
        {
            if (this.SelectedIndex == this.Tweets.Count - 1)
            {
                ExtendToOlder();
            }
            this.SelectedIndex = this.SelectedIndex < (this.Tweets.Count - 1)
                                     ? this.SelectedIndex + 1
                                     : this.SelectedIndex;
        }

        private void ExtendToOlder()
        {
            asyncManager.RunAsync(ExtendToOlder(SelectedTweet));
        }

        private IEnumerable<Action> ExtendToOlder(TweetViewModel tweet)
        {
            IEnumerable<Status> olderStatuses = null;
            yield return () => olderStatuses = _service.GetOlder(tweet.Status);

            AppendStatuses(olderStatuses, tweet);
        }

        private void MoveUp(object obj)
        {
            this.SelectedIndex = this.SelectedIndex > 0
                                     ? (this.SelectedIndex - 1)
                                     : 0;

        }

        private void GetFriendsTimeLine()
        {
            this.asyncManager.RunAsync(GetFriendsTimeLineAsync());
        }

        private IEnumerable<Action> GetFriendsTimeLineAsync()
        {
            IEnumerable<Status> statuses = null;
            yield return () => statuses = this._service.GetLatest();
            this.SetStatuses(statuses);
        }

        private void SetStatuses(IEnumerable<Status> statuses)
        {
            var previousSelected = this.SelectedTweet;

            this.Tweets.Clear();
            AppendStatuses(statuses, previousSelected);
        }

        private void AppendStatuses(IEnumerable<Status> statuses, TweetViewModel previousSelected)
        {
            statuses.Select(s => new TweetViewModel(s)).ForEach(this.Tweets.Add);

            var newSelected = this.Tweets.Count > 0 ? this.Tweets[0] : null;
            if (previousSelected != null)
            {
                newSelected =
                    this.Tweets.FirstOrDefault(
                        t => t.Status.Id == previousSelected.Status.Id);
            }
            this.SelectedTweet = newSelected;
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

        private ObservableCollection<TweetViewModel> _tweets;

        public ObservableCollection<TweetViewModel> Tweets
        {
            get { return _tweets; }
            private set { _tweets = value; }
        }

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
                    CancelEdit(null);

                    selectedTweet = value;
                    this.OnPropertyChanged(p => p.SelectedTweet);

                    this._aggregator.GetEvent<SelectedTweetChangedEvent>()
                        .Publish(value != null ? value.Status : null);
                }
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return _editCommand;
            }
        }

        public ICommand CancelEditCommand
        {
            get {
                return _cancelEditCommand;
            }
        }

        public string Caption { get; private set; }
    }
}
