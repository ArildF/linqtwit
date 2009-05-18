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
        private Mock<ILinqApi> api;
        private Mock<QuerySubmittedEvent> querySubmittedEvent;
        private Mock<AuthorizationStateChangedEvent> authorizationEvent;
        private Mock<RefreshEvent> refreshEvent;
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
            api = this._factory.Create<ILinqApi>();
            this._aggregator = this._factory.Create<IEventAggregator>();
            querySubmittedEvent = new Mock<QuerySubmittedEvent>
                                      {CallBase = true};
            authorizationEvent = this._factory.Create<AuthorizationStateChangedEvent>();
            refreshEvent = this._factory.Create<RefreshEvent>();

            this._aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                this.querySubmittedEvent.Object);
            this._aggregator.Setup(a => a.GetEvent<AuthorizationStateChangedEvent>()).
                Returns(this.authorizationEvent.Object);
            this._aggregator.Setup(a => a.GetEvent<RefreshEvent>()).Returns(
                this.refreshEvent.Object);

            _menuRoot = new ContextMenuRoot();


            asyncManager = new AsyncManager(new MockDispatcherFacade());


            this._vm = new QueryResultsViewModel(view.Object, this._aggregator.Object, api.Object, asyncManager, _menuRoot);
        }

        [Test]
        public void ContextMenu()
        {
            Assert.That(this._vm.ContextMenu, Is.SameAs(_menuRoot));
        }

        [Test]
        public void QuerySubmittedGetsUserTimeline()
        {
            api.Setup(a => a.UserTimeLine("rogue_code")).Returns(new[]
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
            api.Setup(a => a.FriendsTimeLine()).Returns(new[]
                {new Status(), new Status()});

            this.authorizationEvent.Object.Publish(true);

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

            var evt = CreateEvent<SelectedTweetChangedEvent>();

            _vm.SelectedTweet = _vm.Tweets[1];

            evt.Verify(e => e.Publish(It.Is<Status>(s => s.Text == _vm.Tweets[1].Text)));

        }

        private void TestRefresh(bool authorizationState, int count)
        {
            SetupFriendsTimelineCall();

            this.authorizationEvent.Object.Publish(authorizationState);

            this.refreshEvent.Object.Publish(null);

            this.api.Verify(a => a.FriendsTimeLine(), Times.Exactly(count));
        }

        private static void ExecuteMovedown()
        {
            GlobalCommands.DownCommand.Execute(null);
        }

        private void GetStatuses()
        {
            SetupFriendsTimelineCall();

            this.authorizationEvent.Object.Publish(true);
        }

        private void SetupFriendsTimelineCall()
        {
            this.api.Setup(a => a.FriendsTimeLine()).Returns(new[] { new Status(){Text = "tweet 1"}, new Status(){Text = "tweet 2"} });
        }
    }
}
