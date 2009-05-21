using System;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Presentation.Regions.Behaviors;

namespace LinqTwit.Infrastructure.Behaviors
{
    public class DialogRegionBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public const string DialogRegionBehaviorKey = "DialogRegionBehavior";

        private DialogWindow dialog;

        protected override void OnAttach()
        {
            this.Region.ActiveViews.CollectionChanged += ActiveViewsCollectionChanged;
        }

        private void ActiveViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.ShowDialog(e.NewItems[0]);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && this.dialog != null)
            {
                this.CloseDialog();
            }
        }

        private void CloseDialog()
        {
            this.dialog.Close();
        }

        private void ShowDialog(object item)
        {
            this.dialog = new DialogWindow
                {
                    DialogContent = item,
                    Owner = this.HostControl as Window,
                    Style = this.DialogWindowStyle,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };

            this.dialog.Closed += DialogClosed;
            this.dialog.Show();

            this.dialog.Activate();

        }

        protected Style DialogWindowStyle
        {
            get { return (Style) this.HostControl.GetValue(DialogRegionBehaviors.DialogWindowStyleProperty); }
        }

        void DialogClosed(object sender, EventArgs e)
        {
            TerminateDialog();
        }

        private void TerminateDialog()
        {
            this.dialog.Closed -= DialogClosed;
            this.dialog.Content = null;
            this.Region.Deactivate(this.dialog.DialogContent);
            this.dialog = null;

            Application.Current.MainWindow.Activate();
        }

        public DependencyObject HostControl
        {
            get; set;
        }
    }
}
