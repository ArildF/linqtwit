using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqTwit.Twitter;
using NUnit.Framework;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class IdExpressionVisitorTest
    {
        private IdExpressionVisitor _visitor;

        [SetUp]
        public void SetUp()
        {
            _visitor = new IdExpressionVisitor();
        }

        [Test]
        public void TestFoo()
        {
            Visit(tweet => tweet.Id == 1234);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        [Test]
        public void MultipleExpressions()
        {
            Visit(tweet => tweet.Id == 1234 && tweet.Id <= 12345);

            Assert.That(_visitor.Expressions.Count, Is.EqualTo(2));
            Assert.That(_visitor.Expressions[0].NodeType, Is.EqualTo(ExpressionType.Equal));
            Assert.That(_visitor.Expressions[1].NodeType, Is.EqualTo(ExpressionType.LessThanOrEqual));
        }


        [Test]
        public void LocalVariable()
        {
            int local = new Random().Next();
            Visit(tweet => tweet.Id == local);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(local));
        }

        private int Field = 1234;

        [Test]
        public void ReferenceField()
        {
            Visit(tweet => tweet.Id == Field);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private static int StaticField = 1234;

        [Test]
        public void ReferenceStaticField()
        {
            Visit(tweet => tweet.Id == StaticField);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        [Test]
        public void GreaterThanPropertyOfLocal()
        {
            Status status = new Status {Id = 12345};

            Visit(tweet => tweet.Id >= status.Id);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(12345));

        }

        [Test]
        public void EqualsPropertyOfLocal()
        {
            Status status = new Status { Id = 12345 };

            Visit(tweet => tweet.Id == status.Id);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(12345));

        }

        private int AProperty { get; set; }

        [Test]
        public void Property()
        {
            AProperty = 1234;
            Visit(tweet => tweet.Id == AProperty);
            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private static int AStaticProperty { get; set; }

        [Test]
        public void StaticProperty()
        {
            AStaticProperty = 1234;
            Visit(tweet => tweet.Id == AStaticProperty);
            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private class Foo
        {
            public long Bar;
        }

        [Test]
        public void Method()
        {
            Visit(tweet => tweet.Id == AMethod());
            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

// ReSharper disable MemberCanBeMadeStatic.Local
        private int AMethod()
// ReSharper restore MemberCanBeMadeStatic.Local
        {
            return 1234;
        }

        [Test]
        public void MethodWithParams()
        {
            Visit(tweet => tweet.Id == AMethodWithParams(1000, 234));
            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

// ReSharper disable MemberCanBeMadeStatic.Local
        private int AMethodWithParams(int a, int b)
// ReSharper restore MemberCanBeMadeStatic.Local
        {
            return a + b;
        }

        [Test]
        public void StaticMethodWithParams()
        {
            Visit(tweet => tweet.Id == AStaticMethodWithParams(1000, 234));
            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        private static int AStaticMethodWithParams(int a, int b)
        {
            return a + b;
        }

        [Test]
        public void DemeterGoHome()
        {
            var foo = new Foo {Bar = 1234};

            Visit(tweet => tweet.Id == foo.Bar);

            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(1234));
        }

        [Test]
        public void Operators([ValueSource("OperatorExpressions")]Expression<Func<Status, bool>> expression)
        {
            Visit(expression);
            Assert.That(this._visitor.Expressions[0].Right.As<long>(), Is.EqualTo(12345));
        }

// ReSharper disable UnusedMember.Local
        private IEnumerable<Expression<Func<Status, bool>>> OperatorExpressions()
// ReSharper restore UnusedMember.Local
        {
            yield return tweet => tweet.Id >= 12345;
            yield return tweet => tweet.Id <= 12345;
        }

        private void Visit(Expression<Func<Status, bool>> func)
        {
            Assert.That(this._visitor.FindIdExpression(func), Is.True);
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