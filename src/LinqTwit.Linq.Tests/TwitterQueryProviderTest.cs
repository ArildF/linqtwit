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
        private TwitterQueryProvider provider;
        private Mock<ILinqApi> api;
        private readonly MockFactory factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

        [SetUp]
        public void SetUp()
        {
            api = factory.Create<ILinqApi>();
            provider = new TwitterQueryProvider(api.Object, () => new TwitterQuery(api.Object));
        }

        [Test]
        public void TestFoo()
        {

        }

        [Test]
        public void CreateQuery()
        {
            Assert.That(provider.CreateQuery<IUser>(Expression.Constant(String.Empty)), Is.Not.Null);
        }
    }
}