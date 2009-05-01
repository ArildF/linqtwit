using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;

namespace LinqTwit.QueryModule.Views
{
    /// <summary>
    /// Interaction logic for QueryEntryView.xaml
    /// </summary>
    public partial class QueryEntryView : IQueryEntryView
    {
        public QueryEntryView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.eventAggregator = eventAggregator;

            //this.EntryTextBox.DebugEvents("$EVENT {sender.IsEnabled} $STACKTRACE",
            //                              "IsEnabledChanged");

            this.binding = new Binding("ActiveForInput")
                {
                    NotifyOnSourceUpdated = true,
                    NotifyOnTargetUpdated = true,
                    NotifyOnValidationError = true
                };

            Binding.AddSourceUpdatedHandler(this.EntryTextBox, SourceUpdatedHandler);
            Binding.AddTargetUpdatedHandler(this.EntryTextBox, TargetUpdatedHandler);

            this.EntryTextBox.SetBinding(IsEnabledProperty, this.binding);
        }

        private void TargetUpdatedHandler(object sender, DataTransferEventArgs e)
        {
        }

        private void SourceUpdatedHandler(object sender, DataTransferEventArgs args)
        {
            
        }

        private bool isActive;
        private bool hasBeenActivated;
        private readonly IEventAggregator eventAggregator;
        private readonly Binding binding;

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                this.isActive = value;
                if (this.IsActiveChanged != null)
                {
                    this.IsActiveChanged(this, EventArgs.Empty);
                }

                if (this.IsActive)
                {
                    if (!hasBeenActivated)
                    {
                        this.eventAggregator.GetEvent<InitialViewActivatedEvent>().
                            Publish(null);
                        hasBeenActivated = true;
                    }

                }
            }
        }

        public event EventHandler IsActiveChanged;
        public void SetModel(IQueryEntryViewModel model)
        {
            this.DataContext = model;

                {
                    
                }
        }

        private void EntryTextBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
