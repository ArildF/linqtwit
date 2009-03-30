using LinqTwit.QueryModule.ViewModels;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using TestUtilities;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class QueryEntryViewModelTest
    {
        private QueryEntryViewModel vm;
        private readonly MockFactory factory = new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        private Mock<IQueryEntryView> view;

        [SetUp]
        public void SetUp()
        {
            this.view = factory.Create<IQueryEntryView>();

            vm = new QueryEntryViewModel(view.Object);
        }

        [Test]
        public void NotifyPropertyChanged()
        {
            PropertyChangedTester<QueryEntryViewModel> tester = new PropertyChangedTester<QueryEntryViewModel>(vm);

            tester.VerifyAllPublicProperties();
        }

        [Test]
        public void QueryText()
        {
            vm.QueryText = "Hi";
            Assert.That(vm.QueryText, Is.EqualTo("Hi"));
        }
    }
}