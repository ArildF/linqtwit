using System;
using System.Linq;
using System.Linq.Expressions;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class WhereVisitorTest
    {
        private WhereVisitor visitor;
        private Mock<IUser> user;
        private Mock<ILinqApi> api;
        private readonly MockFactory factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };


        [SetUp]
        public void SetUp()
        {
            api = factory.Create<ILinqApi>();
            visitor = new WhereVisitor();
            user = factory.Create<IUser>();

        }

        [Test]
        public void FindsWhereEvenWithSelect()
        {

            var queryable = from u in new Twitter(api.Object).Users
                            where u.Name == "yo"
                            select u.Name;

            MethodCallExpression expr = visitor.FindWhere(queryable.Expression);
            var lambda =
                (LambdaExpression) ((UnaryExpression) expr.Arguments[1]).Operand;

            user.SetupGet(u => u.Name).Returns("yo");

            var ret = lambda.Compile().DynamicInvoke(user.Object);
            Assert.That(ret, Is.True);

            user.SetupGet(u => u.Name).Returns("oy");

            ret = lambda.Compile().DynamicInvoke(user.Object);
            Assert.That(ret, Is.False);


        }

        [Test]
        public void FindsWhereWithoutSelect()
        {

            var queryable = from u in new Twitter(api.Object).Users
                            where u.Name == "yo"
                            select u;

            MethodCallExpression expr = visitor.FindWhere(queryable.Expression);
            Assert.That(expr, Is.Not.Null);
        }
    }
}