using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LinqTwit.Infrastructure.Behaviors;
using LinqTwit.Infrastructure.Commands;
using Microsoft.Practices.Composite.Regions;

namespace LinqTwit.Infrastructure
{
    public interface IDialogService
    {
        void Show(object view);
    }

    public class DialogService : IDialogService
    {
        public void Show(object view)
        {
            var dialog = new DialogWindow
            {
                DialogContent = view, 
                Owner = Application.Current.MainWindow,
                Style = this.DialogWindowStyle, 
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            dialog.ShowDialog();
        }

        protected Style DialogWindowStyle
        {
            get
            {
                return
                    (Style)
                    Application.Current.MainWindow.GetValue(
                        DialogRegionBehaviors.DialogWindowStyleProperty);
            }
        }
    }
}
