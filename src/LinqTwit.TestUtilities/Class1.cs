using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Events;
using Moq;
using NUnit.Framework;

namespace LinqTwit.TestUtilities
{
    [TestFixture]
    public class TestBase
    {
        protected readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose) 
            { DefaultValue = DefaultValue.Mock, CallBase = true};

        protected Mock<IEventAggregator> _aggregator; 


        [SetUp]
        public virtual void Setup()
        {
            _aggregator = _factory.Create<IEventAggregator>();

            OnSetup();
        }

        protected virtual void OnSetup()
        {
            

        }

        protected Mock<T> CreateEvent<T>() where T : EventBase
        {
            var mock = this._factory.Create<T>();

            this._aggregator.Setup(a => a.GetEvent<T>()).Returns(mock.Object);

            return mock;
        }
    }
}
