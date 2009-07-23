using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

using Expr = System.Linq.Expressions.Expression<System.Func<LinqTwit.Linq.ITweet, bool>>;

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
            Visit(tweet => tweet.Id == 1234);

            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        [Test]
        public void MultipleExpressions()
        {
            Visit(tweet => tweet.Id == 1234 && tweet.Id <= 12345);

            Assert.That(visitor.Expressions.Count, Is.EqualTo(2));
            Assert.That(visitor.Expressions[0].NodeType, Is.EqualTo(ExpressionType.Equal));
            Assert.That(visitor.Expressions[1].NodeType, Is.EqualTo(ExpressionType.LessThanOrEqual));
        }


        [Test]
        public void LocalVariable()
        {
            int local = 1234;
            Visit(tweet => tweet.Id == local);

            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private int _field = 1234;
        [Test]
        public void Field()
        {
            Visit(tweet => tweet.Id == _field);

            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private int AProperty { get; set; }

        [Test]
        public void Property()
        {
            AProperty = 1234;
            Visit(tweet => tweet.Id == AProperty);
            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private class Foo
        {
            public long Bar;
        }

        [Test]
        public void Method()
        {
            Visit(tweet => tweet.Id == AMethod());
            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private int AMethod()
        {
            return 1234;
        }

        [Test]
        public void MethodWithParams()
        {
            Visit(tweet => tweet.Id == AMethodWithParams(1000, 234));
            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private int AMethodWithParams(int a, int b)
        {
            return a + b;
        }

        [Test]
        public void DemeterGoHome()
        {
            var foo = new Foo {Bar = 1234};

            Visit(tweet => tweet.Id == foo.Bar);

            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        [Test]
        public void Operators([ValueSource("OperatorExpressions")]Expression<Func<ITweet, bool>> expression)
        {
            Visit(expression);
            Assert.That(visitor.Expressions[0].Right.As<long>(), Is.EqualTo(12345));
        }

        private IEnumerable<Expression<Func<ITweet, bool>>> OperatorExpressions()
        {
            yield return tweet => tweet.Id >= 12345;
            yield return tweet => tweet.Id <= 12345;
        }

        private void Visit(Expression<Func<ITweet, bool>> func)
        {
            Assert.That(visitor.FindIdExpression(func), Is.True);
        }
    }

    static class ExpressionExtensions
    {
        public static T As<T>(this Expression expression)
        {
            return (T) ((ConstantExpression) expression).Value;
        }
    }
}