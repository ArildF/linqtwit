using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqTwit.Common;
using LinqTwit.Core;
using LinqTwit.Infrastructure;
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

        private Mock<QuerySubmittedEvent> _querySubmittedEvent;
        private Mock<RefreshEvent> _refreshEvent;

        private ContextMenuRoot _menuRoot;
        private const string DefaultCaption = "Default";
        private Mock<ITimeLineService> _service;


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

            _service = GetMock<ITimeLineService>();

            
            this.GetMock<IQueryResultsView>();
            this.GetMock<ILinqApi>();
            _aggregator = GetMock<IEventAggregator>();
            _querySubmittedEvent = new Mock<QuerySubmittedEvent>
                                      {CallBase = true};
            _refreshEvent = CreateEvent<RefreshEvent, object>();


            _aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                this._querySubmittedEvent.Object);

            _menuRoot = new ContextMenuRoot();

            Register(_menuRoot);

            Register(DefaultCaption);

            _vm = Create<QueryResultsViewModel>();
        }


        [Test]
        public void ContextMenu()
        {
            Assert.That(this._vm.ContextMenu, Is.SameAs(_menuRoot));
        }

        [Test]
        public void RefreshGetsFriendTimeLine()
        {
            _service.Setup(a => a.GetLatest()).Returns(new[]
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
            TestRefresh(1);
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
            _service.Setup(a => a.GetLatest()).Returns(
                CreateStatuses(1, 10).ToArray());

            _refreshEvent.Object.Publish(true);

            _vm.SelectedTweet = _vm.Tweets[5];
            Assert.That(_vm.SelectedTweet.Status.Id, Is.EqualTo(6));

            _service.Setup(a => a.GetLatest()).Returns(
                CreateStatuses(3, 13).ToArray());

            var tester = new PropertyChangedTester<QueryResultsViewModel>(_vm);
            _refreshEvent.Object.Publish(null);

            Assert.That(_vm.SelectedTweet.Status.Id, Is.EqualTo(6));
            Assert.That(tester.PropertyChanged(p => p.SelectedTweet), Is.True);

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
            _service.Verify(a => a.GetLatest(), Times.Exactly(1));
        }

        [Test]
        public void Caption()
        {
            Register("Default");

            _vm = Create<QueryResultsViewModel>();

            Assert.That(_vm.Caption, Is.EqualTo(DefaultCaption));
        }

        [Test]
        public void GetsOlderWhenMovingPastLastItem()
        {
            GetStatuses();

            var last = _vm.Tweets.Last();

            _vm.SelectedTweet = _vm.Tweets.Last();

            var older = CreateStatuses(3, 4).ToArray();

            _service.Setup(s => s.GetOlder(last.Status)).Returns(older);

            ExecuteMovedown();

            _service.Verify(s => s.GetOlder(last.Status));

            Assert.That(_vm.SelectedTweet.Status, Is.SameAs(older.First()));
        }



        private static IEnumerable<Status> CreateStatuses(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return new Status
                {
                        Id = i,
                        Text = "Some text for status " + i,
                };
            }
        }
        
        private void TestRefresh(int count)
        {
            SetupGetLatestCall(new[] { new Status { Text = "tweet 1" }, new Status { Text = "tweet 2" } });

            _refreshEvent.Object.Publish(null);

            // + 1 to account for the initial call to FriendsTimeLine
            _service.Verify(a => a.GetLatest(), 
                Times.Exactly(count));
        }

        private static void ExecuteMovedown()
        {
            GlobalCommands.DownCommand.Execute(null);
        }

        private void GetStatuses()
        {
            SetupGetLatestCall(CreateStatuses(1, 2));

            _refreshEvent.Object.Publish(true);
        }

        private void SetupGetLatestCall(IEnumerable<Status> statuses)
        {
            _service.Setup(a => a.GetLatest()).Returns(statuses);
        }
    }
}
