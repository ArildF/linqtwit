using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.Tests;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using TestUtilities;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class QueryResultsViewModelTest
    {
        private QueryResultsViewModel vm;

        private Mock<IQueryResultsView> view;
        private Mock<IEventAggregator> aggregator;
        private Mock<ILinqApi> api;
        private Mock<QuerySubmittedEvent> querySubmittedEvent;
        private Mock<AuthorizationStateChangedEvent> authorizationEvent;
        private IAsyncManager asyncManager;

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock, CallBase = true};

        [SetUp]
        public void SetUp()
        {
            view = factory.Create<IQueryResultsView>();
            api = factory.Create<ILinqApi>();
            aggregator = factory.Create<IEventAggregator>();
            querySubmittedEvent = new Mock<QuerySubmittedEvent>
                                      {CallBase = true};
            authorizationEvent = factory.Create<AuthorizationStateChangedEvent>();

            aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                this.querySubmittedEvent.Object);
            aggregator.Setup(a => a.GetEvent<AuthorizationStateChangedEvent>()).
                Returns(this.authorizationEvent.Object);

            asyncManager = new AsyncManager(new MockDispatcherFacade());


            vm = new QueryResultsViewModel(view.Object, aggregator.Object, api.Object, asyncManager);
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

            Assert.That(this.vm.Tweets.Count, Is.EqualTo(2));
        }

        [Test]
        public void AuthenticationStateChangedGetsFriendTimeLine()
        {
            api.Setup(a => a.FriendsTimeLine()).Returns(new[]
                {new Status(), new Status()});

            this.authorizationEvent.Object.Publish(true);

            Assert.That(this.vm.Tweets.Count, Is.EqualTo(2));
        }

        [Test]
        public void SelectedTweet()
        {
            GetStatuses();

            Assert.That(vm.SelectedTweet, Is.SameAs(vm.Tweets[0]));
        }

        [Test]
        public void CheckPropertyChanged()
        {
            GetStatuses();

            PropertyChangedTester<QueryResultsViewModel> tester =
                new PropertyChangedTester<QueryResultsViewModel>(vm);

            vm.SelectedTweet = vm.Tweets[0];

            tester.PropertyChanged(v => v.SelectedTweet);
        }

        [Test]
        public void MoveUp()
        {
            GetStatuses();
            this.vm.SelectedTweet = this.vm.Tweets[1];

            ExecuteMoveUp();

            Assert.That(vm.SelectedTweet, Is.SameAs(vm.Tweets[0]));

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

            Assert.That(vm.SelectedTweet, Is.SameAs(vm.Tweets[1]));
        }

        [Test]
        public void MoveDownRaisesPropertyChanged()
        {
            GetStatuses();

            var tester = new PropertyChangedTester<QueryResultsViewModel>(vm);
            ExecuteMovedown();

            tester.PropertyChanged(v => v.SelectedTweet);

        }

        [Test]
        public void MoveUpRaisesPropertyChanged()
        {
            GetStatuses();
            vm.SelectedTweet = vm.Tweets[1];

            var tester = new PropertyChangedTester<QueryResultsViewModel>(vm);
            ExecuteMoveUp();

            tester.PropertyChanged(v => v.SelectedTweet);

        }

        private static void ExecuteMovedown()
        {
            GlobalCommands.DownCommand.Execute(null);
        }

        private void GetStatuses()
        {
            api.Setup(a => a.FriendsTimeLine()).Returns(new[] { new Status(){Text = "tweet 1"}, new Status(){Text = "tweet 2"} });

            this.authorizationEvent.Object.Publish(true);
        }
    }
}
