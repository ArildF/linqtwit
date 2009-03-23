using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterQueryProviderTest
    {
        private TwitterQueryProvider provider;

        [SetUp]
        public void SetUp()
        {
            provider = new TwitterQueryProvider();
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