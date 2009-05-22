using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LinqTwit.QueryModule.ValueConverters;
using NUnit.Framework;
using Moq;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class UserFullNameConverterTest
    {
        private UserFullNameConverter cut;

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
            cut = new UserFullNameConverter();
        }

        [Test]
        public void Convert()
        {
            Assert.That(cut.Convert("Arild Fines", typeof(string), null, CultureInfo.InvariantCulture), Is.EqualTo(" (Arild Fines) "));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackNotSupported()
        {
            object val = cut.ConvertBack("", typeof (string), null,
                            CultureInfo.InvariantCulture);

        }
    }
}
