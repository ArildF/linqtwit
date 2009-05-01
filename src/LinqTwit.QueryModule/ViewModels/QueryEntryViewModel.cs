﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using LinqTwit.Infrastructure;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Regions;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryEntryViewModel : IQueryEntryViewModel, IRaisePropertyChanged, INotifyPropertyChanged
    {
        private readonly IEventAggregator aggregator;
        public event PropertyChangedEventHandler PropertyChanged;

        public QueryEntryViewModel(IQueryEntryView view, IEventAggregator aggregator, IRegionManager regionManager)
        {
            this.aggregator = aggregator;
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
            this.aggregator.GetEvent<QuerySubmittedEvent>().Publish(this.QueryText);
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

        void IRaisePropertyChanged.RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this,
                                     new PropertyChangedEventArgs(propName));
            }
        }

        private readonly DelegateCommand<object> submitQueryCommand;
        private IRegion region;
        private bool activeForInput;
        private ICommand deactivateCommand;
    }
}
