using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterTest
    {
        private Twitter _twitter;
        private Mock<ILinqApi> api;
        private readonly MockFactory factory = new MockFactory(MockBehavior.Loose){DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
            api = factory.Create<ILinqApi>();

            _twitter = new Twitter(api.Object);
        }

        [Test]
        public void TestFoo()
        {
            
        }

        [Test]
        public void Users()
        {
            Assert.That(_twitter.Users, Is.InstanceOfType(typeof(IQueryable<IUser>  )));
        }

        [Test]
        public void Tweets()
        {
            Assert.That(_twitter.Tweets, Is.InstanceOfType(typeof(IQueryable<ITweet>)));
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
            var status = new Status {Id = "123456"};
            api.Setup(a => a.GetStatus("123456")).Returns(status);

            var tweets = from tweet in _twitter.Tweets
                        where tweet.Id == "123456"
                        select tweet;

            Assert.That(tweets.ToList()[0].Id, Is.EqualTo("123456"));
        }
    }
}