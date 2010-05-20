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
        bool Show(IModalDialog dialog);
    }

    public class DialogService : IDialogService
    {
        private readonly IModalDispatcher _modalDispatcher;
        private readonly IRegionManager _manager;

        public DialogService(IModalDispatcher modalDispatcher, IRegionManager manager)
        {
            _modalDispatcher = modalDispatcher;
            _manager = manager;
        }

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

        public bool Show(IModalDialog dialog)
        {
            var frame = _modalDispatcher.CreateModalFrame();

            dialog.DialogClosed += (sender, args) => frame.Stop();

            _manager.Regions[RegionNames.DialogRegion].Add(dialog);

            _modalDispatcher.Run(frame);

            return dialog.Result ?? false;
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
