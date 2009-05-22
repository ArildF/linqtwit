using System;
using System.ComponentModel;
using Moq;
using NUnit.Framework;
using TestUtilities;

namespace LinqTwit.Utilities.Tests
{
    [TestFixture]
    public class PropertyChangedExtensionsTest
    {
        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TestFoo()
        {

            Changer changer = new Changer();

            PropertyChangedTester<Changer> tester = new PropertyChangedTester<Changer>(changer);

            tester.PropertyChanged(c => c.Text);


        }

        private class Changer : INotifyPropertyChanged, IRaisePropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string text;
            public string Text
            {
                get { return text; }
                set
                {
                    text = value;
                    this.OnPropertyChanged(t => t.Text);
                }
            }

            void IRaisePropertyChanged.RaisePropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }
        }
    }
}
