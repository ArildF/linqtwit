using System.Linq;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterTest
    {
        private Twitter _twitter;
        private Mock<ILinqApi> _api;
        private readonly MockFactory _factory = new MockFactory(MockBehavior.Loose){DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
            _api = this._factory.Create<ILinqApi>();

            _twitter = new Twitter(_api.Object);
        }

        [Test]
        public void TestFoo()
        {
            
        }

        [Test]
        public void Users()
        {
            Assert.That(_twitter.Users, Is.InstanceOf(typeof(IQueryable<IUser>)));
        }

        [Test]
        public void Tweets()
        {
            Assert.That(_twitter.Tweets, Is.InstanceOf(typeof(IQueryable<Status>)));
        }

        [Test]
        public void Query()
        {
            var users = from user in _twitter.Users
                        where user.Name == "rogue_code"
                        select user;
            Assert.That(users.ToList(), Is.Not.Null);
        }

        [Test]
        public void QueryTweetById()
        {
            var status = new Status {Id = 123456};
            _api.Setup(a => a.GetStatus("123456")).Returns(status);

            var tweets = from tweet in _twitter.Tweets
                        where tweet.Id == 123456
                        select tweet;

            Assert.That(tweets.First().Id, Is.EqualTo(123456));
        }
    }
}