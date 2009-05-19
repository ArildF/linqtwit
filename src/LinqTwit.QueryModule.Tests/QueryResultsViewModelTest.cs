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
using NUnit.Framework.SyntaxHelpers;
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
        private Mock<AuthorizationStateChangedEvent> _authorizationEvent;
        private Mock<RefreshEvent> _refreshEvent;
        private IAsyncManager asyncManager;

        private ContextMenuRoot _menuRoot;


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
            this._authorizationEvent = this._factory.Create<AuthorizationStateChangedEvent>();
            this._refreshEvent = CreateEvent<RefreshEvent, object>();


            this._aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                this.querySubmittedEvent.Object);
            this._aggregator.Setup(a => a.GetEvent<AuthorizationStateChangedEvent>()).
                Returns(this._authorizationEvent.Object);

            _menuRoot = new ContextMenuRoot();


            asyncManager = new AsyncManager(new MockDispatcherFacade());


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
            this._api.Setup(a => a.UserTimeLine("rogue_code")).Returns(new[]
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
            this._api.Setup(a => a.FriendsTimeLine()).Returns(new[]
                {new Status(), new Status()});

            this._authorizationEvent.Object.Publish(true);

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
        public void DoesNotRefreshWhenRefreshEventFiredIfNotAuthorized()
        {
            TestRefresh(false, 0);
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
            _api.Setup(a => a.FriendsTimeLine()).Returns(
                CreateStatuses(1, 10).ToArray());

            _authorizationEvent.Object.Publish(true);

            _vm.SelectedTweet = _vm.Tweets[5];

            _api.Setup(a => a.FriendsTimeLine()).Returns(
                CreateStatuses(3, 13).ToArray);

            var tester = new PropertyChangedTester<QueryResultsViewModel>(_vm);
            _refreshEvent.Object.Publish(null);

            Assert.That(tester.PropertyChanged(p => p.SelectedTweet), Is.True);
            Assert.That(_vm.SelectedTweet.Status.Id, Is.EqualTo("5"));
        }



        private static IEnumerable<Status> CreateStatuses(int from, int to)
        {
            for (int i = 0; i <= to; i++)
            {
                yield return new Status()
                    {
                        Id = i.ToString(),
                        Text = "Some text"
                    };
            }
        }
        
        private void TestRefresh(bool authorizationState, int count)
        {
            SetupFriendsTimelineCall();

            this._authorizationEvent.Object.Publish(authorizationState);

            this._refreshEvent.Object.Publish(null);

            // + 1 to account for the initial call to FriendsTimeLine
            this._api.Verify(a => a.FriendsTimeLine(), Times.Exactly(count + (authorizationState ? 1 : 0)));
        }

        private static void ExecuteMovedown()
        {
            GlobalCommands.DownCommand.Execute(null);
        }

        private void GetStatuses()
        {
            SetupFriendsTimelineCall();

            this._authorizationEvent.Object.Publish(true);
        }

        private void SetupFriendsTimelineCall()
        {
            this._api.Setup(a => a.FriendsTimeLine()).Returns(new[] { new Status(){Text = "tweet 1"}, new Status(){Text = "tweet 2"} });
        }
    }
}
