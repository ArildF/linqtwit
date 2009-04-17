using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;

namespace LinqTwit.Infrastructure.Behaviors
{
    public class DialogRegionBehaviors
    {
        /// <summary>
        /// DialogWindowStyle Dependency Property.
        /// </summary>
// ReSharper disable InconsistentNaming
        public static readonly DependencyProperty DialogWindowStyleProperty =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterAttached(
                "DialogWindowStyle",
                typeof (Style),
                typeof (DialogRegionBehaviors),
                new PropertyMetadata(DialogWindowStylePropertyChanged));

        // ReSharper disable InconsistentNaming
        public static readonly DependencyProperty DialogRegionNameProperty =
            // ReSharper restore InconsistentNaming
            DependencyProperty.RegisterAttached(
                "DialogRegionName",
                typeof(string),
                typeof(DialogRegionBehaviors),
                new PropertyMetadata(DialogRegionNameChanged));

        private static void DialogRegionNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (IsInDesignMode(o))
            {
                return;
            }

            var regionManager =
                ServiceLocator.Current.GetInstance<IRegionManager>();
            if (regionManager != null)
            {
                SingleActiveRegion region = new SingleActiveRegion();
                DialogRegionBehavior behavior = new DialogRegionBehavior();

                region.Behaviors.Add(
                    DialogRegionBehavior.DialogRegionBehaviorKey, behavior);
                behavior.HostControl = o;
                var regionName = args.NewValue as string;
                regionManager.Regions.Add(regionName, region);

            }
        }

        private static bool IsInDesignMode(DependencyObject o)
        {
            return DesignerProperties.GetIsInDesignMode(o) || Application.Current == null
                   || Application.Current.GetType() == typeof(Application);
        }

        private static void DialogWindowStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static void SetDialogWindowStyle(DependencyObject obj,Style style)
        {
            obj.SetValue(DialogWindowStyleProperty, style);
        }
        public static void SetDialogRegionName(DependencyObject obj, string name)
        {
            obj.SetValue(DialogRegionNameProperty, name);
        }


    }
}
