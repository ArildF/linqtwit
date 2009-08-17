using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace LinqTwit.Infrastructure.Behaviors
{
    public class ControlBehaviors
    {
        #region DragsWindow dependency property

        static ControlBehaviors()
        {
            //register dependency property
            FrameworkPropertyMetadata md = new FrameworkPropertyMetadata(null, DragsWindowPropertyChanged);
            DragsWindowProperty = DependencyProperty.RegisterAttached("DragsWindow", 
                typeof(Window), typeof(ControlBehaviors), md);  
        }


        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty DragsWindowProperty;

//TODO: copy to static constructor
                                                    


        public static void SetDragsWindow(Control control,
                                Window value)
        {
            control.SetValue(DragsWindowProperty, value);
        }

        public static Window GetDragsWindow(RichTextBox obj)
        {
            return (Window)obj.GetValue(DragsWindowProperty);
        }



        /// <summary>
        /// Handles changes on the <see cref="DragsWindowProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="DragsWindow"/> property wrapper, updates should be handled here.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void DragsWindowPropertyChanged(DependencyObject d,
                                             DependencyPropertyChangedEventArgs
                                                 e)
        {
            FrameworkElement frameworkElement = d as FrameworkElement;
            Window window = e.NewValue as Window;

            if (frameworkElement != null && window != null)
            {
                frameworkElement.MouseDown += (sender, args) => window.DragMove(); 
            }
        }

        #endregion

    }
}
