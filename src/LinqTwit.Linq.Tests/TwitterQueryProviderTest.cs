using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
            provider = new TwitterQueryProvider(api.Object);
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