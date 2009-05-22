using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterQueryableTest
    {
        private TwitterQueryable<IUser> queryable;

        private Mock<ILinqApi> api;
        private readonly MockFactory factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

        [SetUp]
        public void SetUp()
        {
            api = factory.Create<ILinqApi>();

            queryable = new TwitterQueryable<IUser>(api.Object);
        }

        [Test]
        public void ElementType()
        {
            Assert.That(queryable.ElementType, Is.SameAs(typeof(IUser)));
        }

        [Test]
        public void GetEnumerator()
        {
            Assert.That(queryable.GetEnumerator(), Is.Not.Null);
        }

        [Test]
        public void Provider()
        {
            Assert.That(queryable.Provider, Is.TypeOf(typeof(TwitterQueryProvider)));
        }

        [Test]
        public void Expression()
        {
            Assert.That(queryable.Expression, Is.Not.Null);
        }

    }
}