using System;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class IdExpressionVisitorTest
    {
        private IdExpressionVisitor visitor;

        [SetUp]
        public void SetUp()
        {
            visitor = new IdExpressionVisitor();
        }

        [Test]
        public void TestFoo()
        {
            Expression <Func<ITweet, bool>> lambda = user => user.Id == "1234";

            Assert.That(visitor.FindIdExpression(lambda), Is.True);
            Assert.That(visitor.TweetId, Is.EqualTo("1234"));
        }
    }
}