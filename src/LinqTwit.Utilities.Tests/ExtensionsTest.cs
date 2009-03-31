using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using LinqTwit.Utilities;

namespace LinqTwit.Utilities.Tests
{
    [TestFixture]
    public class ExtensionsTest
    {

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void PropertyName()
        {
            Expression<Func<string, int>> func = s => s.Length;

            Assert.That(func.PropertyName(), Is.EqualTo("Length"));
        }

        [Test]
        public void IsProperty()
        {
            
        }


    }
}
