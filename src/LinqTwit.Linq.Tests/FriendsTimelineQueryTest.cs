using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqTwit.Twitter;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class FriendsTimelineQueryTest
    {
        private FriendsTimelineQuery _query;

        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        private Mock<ILinqApi> _api;

        private IQueryable<ITweet> _source;

        [SetUp]
        public void SetUp()
        {
            _api = _factory.Create<ILinqApi>();

            this._query = new FriendsTimelineQuery(_api.Object);

            _source = new TwitterQueryable<ITweet>(_api.Object);

        }

        [Test]
        public void SinceId()
        {
            var args = GetArgs(from t in this._source where t.Id >= 12345 select t);

            Assert.That(args.SinceId, Is.EqualTo(12345));
        }

        [Test] 
        public void MaxId()
        {
            var args = GetArgs(from t in this._source where t.Id <= 12345 select t);

            Assert.That(args.MaxId, Is.EqualTo(12345));
        }

        [Test]
        public void Take()
        {
            var args = GetArgs((from t in _source select t).Take(10));
            Assert.That(args.Count, Is.EqualTo(10));
        }

        private FriendsTimeLineArgs GetArgs(IQueryable<ITweet> queryable)
        {
            FriendsTimeLineArgs args = null;
            this._api.Setup(a => a.FriendsTimeLine(It.IsAny<FriendsTimeLineArgs>())).
                Callback<FriendsTimeLineArgs>(a => args = a).Returns(new Status[]{});

            Expression expr = queryable.Expression  ;

            var results = (IEnumerable<ITweet>)this._query.Execute(expr, true);

            results.ToArray();
            return args;
        }
    }
}
