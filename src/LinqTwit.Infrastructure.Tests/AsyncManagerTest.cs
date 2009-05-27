using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using LinqTwit.TestUtilities;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Infrastructure.Tests
{
    [TestFixture]
    public class AsyncManagerTest
    {
        private AsyncManager mgr;

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        private MockDispatcherFacade mdf = new MockDispatcherFacade();
        private readonly List<int> list = new List<int>();
        private readonly List<int> threadIds = new List<int>();


        [SetUp]
        public void SetUp()
        {
            mgr = new AsyncManager(mdf);
        }

        [Test]
        public void TestFoo()
        {
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;


            mgr.RunAsync(this.RunStuff());

            Assert.That(threadIds.Count, Is.EqualTo(5));
            Assert.That(threadIds[0], Is.EqualTo(mainThreadId));
            Assert.That(threadIds.All(ti => ti == threadIds[0]), Is.True);

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
        }

       

        private IEnumerable<Action> RunStuff()
        {
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Hai");

            threadIds.Add(Thread.CurrentThread.ManagedThreadId);

            yield return () => list.Add(1);

            threadIds.Add(Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Hai hai hai");

            threadIds.Add(Thread.CurrentThread.ManagedThreadId);

            yield return () => list.Add(2);

            threadIds.Add(Thread.CurrentThread.ManagedThreadId);

        }
    }
}
