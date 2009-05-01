using System;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
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
        private Mock<IEventAggregator> aggregator;
        private Mock<ILoginController> controller;
        private Mock<QuerySubmittedEvent> querySubmittedEvent;
        private Mock<InitialViewActivatedEvent> initialViewActivatedEvent;
        private Mock<IRegionManager> regionManager;
        private Mock<IRegion> region;

        [SetUp]
        public void SetUp()
        {
            this.view = factory.Create<IQueryEntryView>();
            this.aggregator = factory.Create<IEventAggregator>();
            this.querySubmittedEvent = factory.Create<QuerySubmittedEvent>();
            this.controller = factory.Create<ILoginController>();
            this.initialViewActivatedEvent = factory.Create<InitialViewActivatedEvent>();
            this.regionManager = factory.Create<IRegionManager>();
            this.region = factory.Create<IRegion>();

            this.regionManager.SetupGet(
                rm => rm.Regions[RegionNames.QueryEntryRegion]).Returns(
                region.Object);


            vm = new QueryEntryViewModel(this.view.Object, this.aggregator.Object, this.regionManager.Object);
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

        [Test]
        public void QueryIsPublishedWhenSubmitted()
        {
            aggregator.Setup(a => a.GetEvent<QuerySubmittedEvent>()).Returns(
                querySubmittedEvent.Object);

            vm.QueryText = "Hai";

            vm.SubmitQueryCommand.Execute(null);

            querySubmittedEvent.Verify(evt => evt.Publish("Hai"));
        }

        [Test]
        public void ActivatedByCommandLineCommand()
        {
            PropertyChangedTester<QueryEntryViewModel> tester = new PropertyChangedTester<QueryEntryViewModel>(vm);
            GlobalCommands.CommandLineCommand.Execute(null);

            region.Verify(r => r.Activate(this.view.Object));

            Assert.That(tester.PropertyChanged(v => v.ActiveForInput), Is.True);
            Assert.That(vm.ActiveForInput, Is.True);
        }

        [Test]
        public void DeactivatedByDeactivateCommand()
        {
            PropertyChangedTester<QueryEntryViewModel> tester =
                new PropertyChangedTester<QueryEntryViewModel>(vm);

            vm.DeactivateCommand.Execute();

            Assert.That(tester.PropertyChanged(v => v.ActiveForInput), Is.True);
            Assert.That(vm.ActiveForInput, Is.False);
        }

       
    }
}