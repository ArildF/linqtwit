using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.QueryModule;
using Microsoft.Practices.Composite.Events;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Commands.Tests
{
    [TestFixture]
    public class RefreshCommandTest
    {
        private RefreshCommand _refreshCommand;

        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock, CallBase = true};

        private Mock<RefreshEvent> _refreshEvent;
        private Mock<IEventAggregator> _aggregator;

        [SetUp]
        public void SetUp()
        {
            _refreshEvent = this._factory.Create<RefreshEvent>();

            this._aggregator = this._factory.Create<IEventAggregator>();
            this._aggregator.Setup(a => a.GetEvent<RefreshEvent>()).Returns(
                _refreshEvent.Object);
            this._refreshCommand = new RefreshCommand(this._aggregator.Object);
        }

        [Test]
        public void CanExecuteAlwaysTrue()
        {
            Assert.That(this._refreshCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void ExecuteRaisesRefreshEvent()
        {
            bool raised = false;
            _refreshEvent.Object.Subscribe(_ => raised = true);

            _refreshCommand.Execute(null);

            Assert.That(raised, Is.True);

        }
    }
}
