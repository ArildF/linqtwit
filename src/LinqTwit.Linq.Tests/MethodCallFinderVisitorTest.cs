using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using LinqTwit.Twitter;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class MethodCallFinderVisitorTest
    {
        private MethodCallFinderVisitor _visitor;

        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        private IEnumerable<int> _enumerable = new int[] {1, 2, 3, 4};
        private Mock<ILinqApi> _api;
        private IQueryable<ITweet> _queryable;

        [SetUp]
        public void SetUp()
        {
            _api = _factory.Create<ILinqApi>();
            _queryable = new Twitter(_api.Object).Tweets;
        }

        [Test]
        public void TestFoo()
        {
            MethodInfo info = LinqTwit.Utilities.Extensions.MethodOf<IQueryable<ITweet>, IQueryable<ITweet>>(
                q => q.Take(10));

            _visitor =
                new MethodCallFinderVisitor(info);

            Visit ((from i in _queryable select i).Take(10));

            Assert.That(_visitor.Args.First(), Is.EqualTo(10));

        }

        private void Visit<T>(IQueryable<T> queryable)
        {
            Assert.That(_visitor.FindMethod(queryable.Expression), Is.True);
        }
    }
}
