using System;
using System.Linq.Expressions;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterQueryProviderTest
    {
        private TwitterQueryProvider _provider;
        private Mock<ILinqApi> _api;
        private readonly MockFactory _factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

        [SetUp]
        public void SetUp()
        {
            _api = _factory.Create<ILinqApi>();
            _provider = new TwitterQueryProvider(_api.Object, () => new TwitterQuery(_api.Object));
        }

        [Test]
        public void TestFoo()
        {

        }

        [Test]
        public void CreateQuery()
        {
            Assert.That(_provider.CreateQuery<IUser>(Expression.Constant(String.Empty)), Is.Not.Null);
        }
    }
}