using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqTwit.Twitter;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TimelineQueryTest
    {
        private TimelineQuery _query;

        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        private Mock<ILinqApi> _api;

        private IQueryable<Status> _source;

        [SetUp]
        public void SetUp()
        {
            _api = _factory.Create<ILinqApi>();

            this._query = new TimelineQuery(_api.Object);

            ILinqApi linqApi = _api.Object;
            _source = new TwitterQueryable<Status>(new TwitterQueryProvider(() => new TwitterQuery(linqApi)));

        }

        [Test]
        public void SinceId()
        {
            var args = GetArgs(from t in this._source where t.Id >= 12345 select t);

            Assert.That(args.SinceId, Is.EqualTo(12345));
        }

        [Test]
        public void SinceIdFromNonLocal()
        {
            Status oldStatus = new Status {Id = 12345};

            var args = GetArgs(from t in this._source where t.Id >= oldStatus.Id select t);

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

        [Test]
        public void Page()
        {
            var args = GetArgs((from t in _source select t).Page(2));
            Assert.That(args.Page, Is.EqualTo(2));
        }

        private TimeLineArgs GetArgs(IQueryable<Status> queryable)
        {
            TimeLineArgs args = null;
            this._api.Setup(a => a.FriendsTimeLine(It.IsAny<TimeLineArgs>())).
                Callback<TimeLineArgs>(a => args = a).Returns(new Status[]{});

            Expression expr = queryable.Expression  ;

            var results = (IEnumerable<Status>)this._query.Execute(expr, true);

            results.ToArray();
            return args;
        }
    }
}
