using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinqTwit.Common;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.Tests;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.TestUtilities;
using LinqTwit.Twitter;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using NUnit.Framework;
using Moq;
using TestUtilities;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class QueryResultsViewModelTest : TestBase
    {
        private QueryResultsViewModel _vm;

        private Mock<IQueryResultsView> view;
        private Mock<ILinqApi> _api;
        private Mock<QuerySubmittedEvent> querySubmittedEvent;
        private Mock<RefreshEvent> _refreshEvent;
        private IAsyncManager asyncManager;

        private ContextMenuRoot _menuRoot;
        private MockDispatcherFacade _dispatcherFacade;


        protected override void  OnSetup()
        {
            var commands =
                from prop in
                    typeof (GlobalCommands).GetProperties(BindingFlags.Static |
                                                          BindingFlags.Public)
                where
                    typeof(CompositeCommand).IsAssignableFrom(prop.PropertyType)
                select prop.GetValue(null, null) as CompositeCommand;
            foreach (var command in commands)
            {
                command.RegisteredCommands.ForEach(command.UnregisterCommand);
            }

            
            view = this._factory.Create<IQueryResultsView>();
            this._api = this._factory.Create<ILinqApi>();
            this._aggregator = _factory.Create<IEventAggregator>();
            querySubmittedEvent = new Mock<QuerySubmittedEvent>
                                      {CallBase = true};
            this._refreshEvent = CreateEvent<RefreshEvent, object>();


            this._aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                this.querySubmittedEvent.Object);

            _menuRoot = new ContextMenuRoot();


            _dispatcherFacade = new MockDispatcherFacade();
            asyncManager = new AsyncManager(this._dispatcherFacade);


            this._vm = new QueryResultsViewModel(view.Object, this._aggregator.Object, this._api.Object, asyncManager, _menuRoot);
        }


        [Test]
        public void ContextMenu()
        {
            Assert.That(this._vm.ContextMenu, Is.SameAs(_menuRoot));
        }

        [Test]
        public void QuerySubmittedGetsUserTimeline()
        {
            this._api.Setup(a => a.UserTimeLine("rogue_code", It.IsAny<TimeLineArgs>())).Returns(new[]
                                                                     {
                                                                         new Status
                                                                             (),
                                                                         new Status
                                                                             ()
                                                                     });

            this.querySubmittedEvent.Object.Publish("rogue_code");

            Assert.That(this._vm.Tweets.Count, Is.EqualTo(2));
        }

        [Test]
        public void AuthenticationStateChangedGetsFriendTimeLine()
        {
            this._api.Setup(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>())).Returns(new[]
                {new Status(), new Status()});

            _refreshEvent.Object.Publish(null);

            Assert.That(this._vm.Tweets.Count, Is.EqualTo(2));
        }

        [Test]
        public void SelectedTweet()
        {
            GetStatuses();

            Assert.That(this._vm.SelectedTweet, Is.SameAs(this._vm.Tweets[0]));
        }

        [Test]
        public void CheckPropertyChanged()
        {
            GetStatuses();

            PropertyChangedTester<QueryResultsViewModel> tester =
                new PropertyChangedTester<QueryResultsViewModel>(this._vm);

            this._vm.SelectedTweet = this._vm.Tweets[0];

            tester.PropertyChanged(v => v.SelectedTweet);
        }

        [Test]
        public void MoveUp()
        {
            GetStatuses();
            this._vm.SelectedTweet = this._vm.Tweets[1];

            ExecuteMoveUp();

            Assert.That(this._vm.SelectedTweet, Is.SameAs(this._vm.Tweets[0]));

        }

        private static void ExecuteMoveUp()
        {
            GlobalCommands.UpCommand.Execute(null);
        }

        [Test]
        public void MoveDown()
        {
            GetStatuses();

            ExecuteMovedown();

            Assert.That(this._vm.SelectedTweet, Is.SameAs(this._vm.Tweets[1]));
        }

        [Test]
        public void MoveDownRaisesPropertyChanged()
        {
            GetStatuses();

            var tester = new PropertyChangedTester<QueryResultsViewModel>(this._vm);
            ExecuteMovedown();

            tester.PropertyChanged(v => v.SelectedTweet);

        }

        [Test]
        public void MoveUpRaisesPropertyChanged()
        {
            GetStatuses();
            this._vm.SelectedTweet = this._vm.Tweets[1];

            var tester = new PropertyChangedTester<QueryResultsViewModel>(this._vm);
            ExecuteMoveUp();

            tester.PropertyChanged(v => v.SelectedTweet);

        }

        [Test]
        public void RefreshesWhenRefreshEventFired()
        {
            TestRefresh(true, 1);
        }

     
        [Test]
        public void RaisesSelectedTweetChangedEvent()
        {
            GetStatuses();

            var evt = CreateEvent<SelectedTweetChangedEvent, Status>();

            _vm.SelectedTweet = _vm.Tweets[1];

            evt.Verify(e => e.Publish(It.Is<Status>(s => s.Text == _vm.Tweets[1].Text)));
        }

        [Test]
        public void SelectedTweetIsRetainedAfterRefresh()
        {
            _api.Setup(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>())).Returns(
                CreateStatuses(1, 10).ToArray());

            _refreshEvent.Object.Publish(true);

            _vm.SelectedTweet = _vm.Tweets[5];
            Assert.That(_vm.SelectedTweet.Status.Id, Is.EqualTo(6));

            _api.Setup(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>())).Returns(
                CreateStatuses(3, 13).ToArray());

            var tester = new PropertyChangedTester<QueryResultsViewModel>(_vm);
            _refreshEvent.Object.Publish(null);

            Assert.That(tester.PropertyChanged(p => p.SelectedTweet), Is.True);
            Assert.That(_vm.SelectedTweet.Status.Id, Is.EqualTo(6));
        }

        [Test]
        public void Editable()
        {
            GetStatuses();

            _vm.SelectedTweet = _vm.Tweets[1];

            Assert.That(_vm.EditCommand.CanExecute(null), Is.True);

            _vm.SelectedTweet = null;
            Assert.That(_vm.EditCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void Edit()
        {
            GetStatuses();

            _vm.SelectedTweet = _vm.Tweets[1];

            _vm.EditCommand.Execute(null);

            Assert.That(_vm.Tweets[1].Editable, Is.True);
        }

        [Test]
        public void CancelEditCanExecute()
        {
            GetStatuses();

            _vm.SelectedTweet = _vm.Tweets[1];
            _vm.SelectedTweet.Editable = false;

            Assert.That(_vm.CancelEditCommand.CanExecute(null), Is.False);

            _vm.SelectedTweet.Editable = true;
            Assert.That(_vm.CancelEditCommand.CanExecute(null), Is.True);

            _vm.SelectedTweet = null;
            Assert.That(_vm.CancelEditCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void CancelEdit()
        {
            GetStatuses();
            _vm.SelectedTweet = _vm.Tweets[1];

            _vm.SelectedTweet.Editable = true;

            _vm.CancelEditCommand.Execute(null);

            Assert.That(_vm.SelectedTweet.Editable, Is.False);
        }

        [Test]
        public void EditCancelledWhenTweetUnselected()
        {
            GetStatuses();
            _vm.SelectedTweet = _vm.Tweets[1];
            _vm.SelectedTweet.Editable = true;

            _vm.SelectedTweet = _vm.Tweets[0];
            Assert.That(_vm.Tweets[1].Editable, Is.False);
        }

        [Test]
        public void DoesNotRefreshWhenSelectedTweetEditable()
        {
            GetStatuses();
            _vm.SelectedTweet = _vm.Tweets[1];
            _vm.SelectedTweet.Editable = true;

            _refreshEvent.Object.Publish(null);

            // 1 for the initial call in GetStatuses()
            _api.Verify(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>()), Times.Exactly(1));
        }



        private static IEnumerable<Status> CreateStatuses(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return new Status()
                    {
                        Id = i,
                        Text = "Some text"
                    };
            }
        }
        
        private void TestRefresh(bool authorizationState, int count)
        {
            SetupFriendsTimelineCall();

            _refreshEvent.Object.Publish(null);

            // + 1 to account for the initial call to FriendsTimeLine
            this._api.Verify(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>()), 
                Times.Exactly(count));
        }

        private static void ExecuteMovedown()
        {
            GlobalCommands.DownCommand.Execute(null);
        }

        private void GetStatuses()
        {
            SetupFriendsTimelineCall();

            _refreshEvent.Object.Publish(true);
        }

        private void SetupFriendsTimelineCall()
        {
            this._api.Setup(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>())).Returns(new[] { new Status() { Text = "tweet 1" }, new Status() { Text = "tweet 2" } });
        }
    }
}
