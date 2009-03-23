using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterQueryableTest
    {
        private TwitterQueryable<IUser> queryable;

        [SetUp]
        public void SetUp()
        {
            queryable = new TwitterQueryable<IUser>();
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