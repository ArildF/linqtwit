using System;
using System.ComponentModel;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.Commands;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Regions;

namespace LinqTwit.QueryModule.ViewModels
{
   

    public class QueryEntryViewModel : ViewModelBase, IQueryEntryViewModel
    {
        private readonly IEventAggregator aggregator;
        private readonly ICommandExecutor _executor;

        public QueryEntryViewModel(IQueryEntryView view, IEventAggregator aggregator, IRegionManager regionManager, 
            ICommandExecutor executor)
        {
            this.aggregator = aggregator;
            _executor = executor;
            View = view;
            View.SetModel(this);

            this.region = regionManager.Regions[RegionNames.QueryEntryRegion];


            this.submitQueryCommand =
                new DelegateCommand<object>(OnSubmitQuery, o => !String.IsNullOrEmpty(QueryText));
            this.deactivateCommand = new DelegateCommand<object>(OnDeactivate);

            GlobalCommands.CommandLineCommand.RegisterCommand(new DelegateCommand<object>(CommandLineExecuted));
        }

        private void OnDeactivate(object obj)
        {
            this.ActiveForInput = false;
        }

        private void CommandLineExecuted(object obj)
        {
            this.region.Activate(this.View);
            this.ActiveForInput = true;
        }

        private void OnSubmitQuery(object obj)
        {
            _executor.Execute(QueryText);

            this.QueryText = String.Empty;
            this.ActiveForInput = false;
        }

        public IQueryEntryView View
        {
            get; private set;
        }

        private string queryText;

        public string QueryText
        {
            get { return queryText; }
            set
            {
                queryText = value;
                this.OnPropertyChanged(x => x.QueryText);
                this.submitQueryCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand SubmitQueryCommand
        {
            get { return submitQueryCommand; }
        }

        public bool ActiveForInput
        {
            get {
                return activeForInput;
            }
            set
            {
                if (activeForInput != value)
                {
                    activeForInput = value;
                    this.OnPropertyChanged(vm => vm.ActiveForInput);
                }
            }
        }

        public ICommand DeactivateCommand
        {
            get
            {
                return deactivateCommand;
            }
        }

        private readonly DelegateCommand<object> submitQueryCommand;
        private IRegion region;
        private bool activeForInput;
        private ICommand deactivateCommand;
    }
}
