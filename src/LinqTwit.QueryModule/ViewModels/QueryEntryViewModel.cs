using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryEntryViewModel : IQueryEntryViewModel, IRaisePropertyChanged, INotifyPropertyChanged
    {
        private readonly IEventAggregator aggregator;
        private readonly ILoginController controller;
        public event PropertyChangedEventHandler PropertyChanged;

        public QueryEntryViewModel(IQueryEntryView view, IEventAggregator aggregator, ILoginController controller)
        {
            this.aggregator = aggregator;
            this.controller = controller;
            View = view;
            View.SetModel(this);

            this.submitQueryCommand =
                new DelegateCommand<object>(OnSubmitQuery, o => !String.IsNullOrEmpty(QueryText));

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

        void IRaisePropertyChanged.RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this,
                                     new PropertyChangedEventArgs(propName));
            }
        }

        private readonly DelegateCommand<object> submitQueryCommand;
    }
}
