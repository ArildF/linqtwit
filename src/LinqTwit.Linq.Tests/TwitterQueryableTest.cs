using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterQueryableTest
    {
        private TwitterQueryable<IUser> _queryable;

        private Mock<ILinqApi> _api;
        private readonly MockFactory _factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

        [SetUp]
        public void SetUp()
        {
            _api = _factory.Create<ILinqApi>();

            _queryable = new TwitterQueryable<IUser>(_api.Object);
        }

        [Test]
        public void ElementType()
        {
            Assert.That(_queryable.ElementType, Is.SameAs(typeof(IUser)));
        }

        [Test]
        public void GetEnumerator()
        {
            Assert.That(_queryable.GetEnumerator(), Is.Not.Null);
        }

        [Test]
        public void Provider()
        {
            Assert.That(_queryable.Provider, Is.TypeOf(typeof(TwitterQueryProvider)));
        }

        [Test]
        public void Expression()
        {
            Assert.That(_queryable.Expression, Is.Not.Null);
        }

    }
}