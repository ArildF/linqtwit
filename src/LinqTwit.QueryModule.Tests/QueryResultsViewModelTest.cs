using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;

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

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
            view = factory.Create<IQueryResultsView>();
            api = factory.Create<ILinqApi>();
            aggregator = factory.Create<IEventAggregator>();
            querySubmittedEvent = new Mock<QuerySubmittedEvent>
                                      {CallBase = true};


            aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                this.querySubmittedEvent.Object);

            vm = new QueryResultsViewModel(view.Object, aggregator.Object, api.Object);
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

            Assert.That(this.vm.Statuses.Count, Is.EqualTo(2));
        }
    }
}
