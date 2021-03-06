using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Core;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule.Controllers;
using LinqTwit.TestUtilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
using NUnit.Framework;
using Moq;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class TweetScreenControllerTest : TestBase
    {
        private TweetScreenController _controller;

        private Mock<AuthorizationStateChangedEvent> _authorizationEvent;
        private Mock<RefreshEvent> _refreshEvent;
        private Mock<ITweetScreenFactory> _screenFactory;
        private Mock<IEventAggregator> _foo;

        protected override void OnSetup()
        {
            _screenFactory = GetMock<ITweetScreenFactory>();

            _foo = GetMock<IEventAggregator>();

            _authorizationEvent = CreateEvent<AuthorizationStateChangedEvent, bool>();
            _refreshEvent = CreateEvent<RefreshEvent, object>();

            _controller = Create<TweetScreenController>();
            
        }

        [Test]
        public void CreatesDefaultViewWhenLoggedIn()
        {
            var vm = VerifyViewCreatedOnLoggedIn(GetMock<ITimeLineFactory>().Object.CreateFriendsTimeLine(), "Main timeline");
            GetMock<IRegion>().Verify(r => r.Activate(vm.Object.View));}

        [Test]
        public void RefreshesAfterCreate()
        {
            bool refreshed = false;
            _refreshEvent.Object.Subscribe(_ => refreshed = true);

            _authorizationEvent.Object.Publish(true);

            Assert.That(refreshed, Is.True);
        }

        [Test]
        public void ActivatesAfterCreate()
        {
            _authorizationEvent.Object.Publish(true);

            IQueryResultsView view = GetMock<IQueryResultsViewModel>().Object.View;
            
        }

        [Test]
        public void CreatesMentionsViewWhenLoggedIn()
        {
            VerifyViewCreatedOnLoggedIn(GetMock<ITimeLineFactory>().Object.CreateMentionsTimeLine(), "Mentions");
        }

        private Mock<IQueryResultsViewModel> VerifyViewCreatedOnLoggedIn(ITimeLineService timeLine, string title)
        {
            var viewModel = GetMock<IQueryResultsViewModel>();

            _screenFactory.Setup(f => f.Create(title, timeLine)).Returns(viewModel.Object);

            _authorizationEvent.Object.Publish(true);

            _screenFactory.Verify(f => f.Create(title, timeLine));

            Mock<IRegion> region = GetMock<IRegion>();
            region.Verify(rf => rf.Add(viewModel.Object.View));

            return viewModel;
            
        }
    }
}
