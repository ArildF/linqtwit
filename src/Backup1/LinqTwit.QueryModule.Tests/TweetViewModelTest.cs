using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.Twitter;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using TestUtilities;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class TweetViewModelTest
    {
        private Status status;
        private User user;
        private TweetViewModel vm;

        [SetUp]
        public void SetUp()
        {
            status = new Status();
            user = status.User = new User();

            vm = new TweetViewModel(status);
        }

        [Test]
        public void TestFoo()
        {

        }

        [Test]
        public void NotifyPropertyChanged()
        {
            PropertyChangedTester<TweetViewModel> tester =
                new PropertyChangedTester<TweetViewModel>(vm);
            tester.VerifyAllPublicProperties();
        }

        [Test]
        public void PropertyChanged()
        {
            Assert.That(vm, Is.InstanceOfType(typeof(INotifyPropertyChanged)));
        }

        [Test]
        public void StatusProperties()
        {
            var now = DateTime.Now;
            status.SetCreatedAt(now);
            status.Text = "Yo";

            user.Name = "Arild Fines";
            user.ScreenName = "rogue_code";
            user.ProfileImageUrl = "http://whatever";


            Assert.That(vm.Text, Is.EqualTo("Yo"));
            Assert.That(vm.Created, Is.EqualTo(now.ToTwitterAccuracy()));
            Assert.That(vm.FullName, Is.EqualTo("Arild Fines"));
            Assert.That(vm.ScreenName, Is.EqualTo("rogue_code"));
            Assert.That(vm.ProfileImageUrl, Is.EqualTo("http://whatever"));
        }
    }

    public static class StatusExtensions
    {
        public static void SetCreatedAt(this Status status, DateTime dt)
        {
            status.GetType().GetField("createdAt", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(status, dt.ToString("ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture));
        }

        public static DateTime ToTwitterAccuracy(this DateTime dt)
        {
            return DateTime.ParseExact(dt.ToString("ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture),
                "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture);
        }
    }
}
