using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
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

        protected struct Subscription<T>
        {
            public Action<T> Handler;
            public Predicate<T> Filter;
        }

        protected T CreateUnmockedEvent<T>() where T: EventBase, new()
        {
            T evt = new T();

      
            _aggregator.Setup(a => a.GetEvent<T>()).Returns(evt);

            return evt;
        }

        protected Mock<TEvent> CreateEvent<TEvent, TPayload>() where TEvent : CompositePresentationEvent<TPayload>
        {
            var mock = this._factory.Create<TEvent>();

            var listeners = new List<Subscription<TPayload>>();

            _aggregator.Setup(a => a.GetEvent<TEvent>()).Returns(mock.Object);

            mock.Setup(m => m.Subscribe(
                                It.IsAny<Action<TPayload>>(),
                                It.IsAny<ThreadOption>(), It.IsAny<bool>(),
                                It.IsAny<Predicate<TPayload>>()))
                .Callback
                <Action<TPayload>, ThreadOption, bool, Predicate<TPayload>>
                ((action, option, keepAlive, filter) =>
                 listeners.Add(new Subscription<TPayload>
                     {Filter = filter, Handler = action}));
            mock.Setup(m => m.Publish(It.IsAny<TPayload>())).Callback<TPayload>(
                pl =>
                listeners.Where(l => l.Filter(pl)).ForEach(l => l.Handler(pl)));

            return mock;
        }
    }
}
