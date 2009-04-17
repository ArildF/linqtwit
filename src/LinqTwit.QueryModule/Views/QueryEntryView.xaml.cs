using System;
using System.Windows.Data;
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
        }

        private bool isActive;
        private bool hasBeenActivated;
        private readonly IEventAggregator eventAggregator;

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

                if (this.IsActive && !hasBeenActivated)
                {
                    this.eventAggregator.GetEvent<InitialViewActivatedEvent>().
                        Publish(null);
                    hasBeenActivated = true;

                }
            }
        }

        public event EventHandler IsActiveChanged;
        public void SetModel(IQueryEntryViewModel model)
        {
            this.DataContext = model;
        }
    }
}
