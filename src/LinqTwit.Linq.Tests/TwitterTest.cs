using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Linq.Tests
{
    [TestFixture]
    public class TwitterTest
    {
        private Twitter _twitter;

        [SetUp]
        public void SetUp()
        {
            _twitter = new Twitter();
        }

        [Test]
        public void TestFoo()
        {
            
        }

        [Test]
        public void Users()
        {
            Assert.That(_twitter.Users, Is.Not.Null);
        }

        [Test]
        public void Query()
        {
            var users = from user in _twitter.Users
                        where user.Name == "rogue_code"
                        select user;
            Assert.That(users.ToList(), Is.Not.Null);
            ;

        }
    }
}