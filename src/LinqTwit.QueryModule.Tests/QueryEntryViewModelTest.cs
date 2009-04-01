using System;
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

        [Test]
        public void SubmitQueryDisabledWhenQueryTextEmpty()
        {
            vm.QueryText = String.Empty;
            Assert.That(vm.SubmitQueryCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void SubmitQueryEnabledWhenQueryTextSet()
        {
            vm.QueryText = "Hai";
            Assert.That(vm.SubmitQueryCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void SettingQueryTextRaiseCanExecuteChangedOnSubmitQueryCommand()
        {
            bool raised = false;
            vm.SubmitQueryCommand.CanExecuteChanged += (_1, _2) => raised = true;

            vm.QueryText = "Hai";
            Assert.That(raised, Is.True);

        }
    }
}