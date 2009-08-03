using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Moq;
using LinqTwit.Utilities;

namespace LinqTwit.Utilities.Tests
{
    [TestFixture]
    public class ExtensionsTest
    {

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void PropertyName()
        {
            Expression<Func<string, int>> func = s => s.Length;

            Assert.That(func.PropertyName(), Is.EqualTo("Length"));
        }

        [Test]
        public void IsProperty()
        {
            
        }

        [Test]
        public void DebugEvents()
        {
            ClassWithEvent c = new ClassWithEvent();
            c.DebugEvents("Yohoo", ".*");

            c.RaiseEvent();
        }

        [Test]
        public void MethodOf()
        {
            IQueryable<string> queryable = null;
            MethodInfo info = queryable.MethodOf(q => q.Take(10));
            Assert.That(info.Name, Is.EqualTo("Take"));
        }

        class ClassWithEvent
        {
            public event EventHandler Event;

            public void RaiseEvent()
            {
                if (this.Event != null)
                {
                    this.Event(this, EventArgs.Empty);
                }
            }
        }


    }
}
