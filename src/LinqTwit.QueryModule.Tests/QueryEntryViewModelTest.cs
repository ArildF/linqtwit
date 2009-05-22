using System;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.Commands;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
using Moq;
using NUnit.Framework;
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
        private Mock<ICommandExecutor> _executor;
        private PropertyChangedTester<QueryEntryViewModel> _tester;

        public QueryEntryViewModelTest()
        {
            
        }

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

            _executor = factory.Create<ICommandExecutor>();


            this.regionManager.SetupGet(
                rm => rm.Regions[RegionNames.QueryEntryRegion]).Returns(
                region.Object);


            vm = new QueryEntryViewModel(this.view.Object, this.aggregator.Object, 
                this.regionManager.Object, _executor.Object);

            _tester = new PropertyChangedTester<QueryEntryViewModel>(this.vm);
        }

        [Test]
        public void NotifyPropertyChanged()
        {
            PropertyChangedTester<QueryEntryViewModel> tester = new PropertyChangedTester<QueryEntryViewModel>(vm);

            vm.ActiveForInput = true;

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
        public void ActivatedByCommandLineCommand()
        {
            GlobalCommands.CommandLineCommand.Execute(null);

            region.Verify(r => r.Activate(this.view.Object));

            Assert.That(this._tester.PropertyChanged(v => v.ActiveForInput), Is.True);
            Assert.That(vm.ActiveForInput, Is.True);
        }

        [Test]
        public void DeactivatedByDeactivateCommand()
        {
            vm.ActiveForInput = true;

            vm.DeactivateCommand.Execute();

            Assert.That(_tester.PropertyChanged(v => v.ActiveForInput), Is.True);
            Assert.That(vm.ActiveForInput, Is.False);
        }

        [Test]
        public void ExecuteCommand()
        {
            InvokeCommand("Foo.Command");

            _executor.Verify(e => e.Execute("Foo.Command"));
        }

        [Test]
        public void DeactivatedAfterExecuteCommand()
        {
            vm.ActiveForInput = true;

            InvokeCommand("Command");

            Assert.That(_tester.PropertyChanged(m => m.ActiveForInput), Is.True);
            Assert.That(vm.ActiveForInput, Is.False);
        }

        [Test]
        public void QueryClearedAfterExecuting()
        {
            InvokeCommand("Command");
            Assert.That(vm.QueryText, Is.Empty);
        }

        private void InvokeCommand(string command)
        {
            this.vm.QueryText = command;
            this.vm.SubmitQueryCommand.Execute(null);
        }
    }
}