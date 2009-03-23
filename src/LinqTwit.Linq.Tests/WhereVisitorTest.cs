using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class WhereVisitorTest
    {
        private WhereVisitor visitor;
        private Mock<IUser> user;

        private readonly MockFactory factory = new MockFactory(MockBehavior.Loose){DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
            visitor = new WhereVisitor();
            user = factory.Create<IUser>();

        }

        [Test]
        public void TestFoo()
        {

            var queryable = from u in new Twitter().Users
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
    }
}