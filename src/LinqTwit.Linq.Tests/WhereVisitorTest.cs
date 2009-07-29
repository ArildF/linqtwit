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
        private WhereVisitor _visitor;
        private Mock<IUser> _user;
        private Mock<ILinqApi> _api;
        private readonly MockFactory _factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };


        [SetUp]
        public void SetUp()
        {
            _api = _factory.Create<ILinqApi>();
            _visitor = new WhereVisitor();
            _user = _factory.Create<IUser>();

        }

        [Test]
        public void FindsWhereEvenWithSelect()
        {

            var queryable = from u in new Twitter(_api.Object).Users
                            where u.Name == "yo"
                            select u.Name;

            MethodCallExpression expr = _visitor.FindWhere(queryable.Expression);
            var lambda =
                (LambdaExpression) ((UnaryExpression) expr.Arguments[1]).Operand;

            _user.SetupGet(u => u.Name).Returns("yo");

            var ret = lambda.Compile().DynamicInvoke(_user.Object);
            Assert.That(ret, Is.True);

            _user.SetupGet(u => u.Name).Returns("oy");

            ret = lambda.Compile().DynamicInvoke(_user.Object);
            Assert.That(ret, Is.False);


        }

        [Test]
        public void FindsWhereWithoutSelect()
        {

            var queryable = from u in new Twitter(_api.Object).Users
                            where u.Name == "yo"
                            select u;

            MethodCallExpression expr = _visitor.FindWhere(queryable.Expression);
            Assert.That(expr, Is.Not.Null);
        }
    }
}