using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LinqTwit.Infrastructure
{
    public class ListBoxBehavior
    {
        #region KeepSelectionInView Dependency Property


        /// <summary>
        /// KeepSelectionInView Dependency Property.
        /// </summary>
        public static readonly DependencyProperty KeepSelectionInViewProperty =
            DependencyProperty.Register(
                "KeepSelectionInView",
                typeof (bool),
                typeof (ListBox),
                new PropertyMetadata(new PropertyChangedCallback(KeepSelectionInViewChanged)));

        public static void SetKeepSelectionInView(DependencyObject obj,
                                                  bool value)
        {
            obj.SetValue(KeepSelectionInViewProperty, value);
        }

        public static bool GetKeepSelectionInView(DependencyObject obj)
        {
            return (bool) obj.GetValue(KeepSelectionInViewProperty);
        }

        private static void KeepSelectionInViewChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            ListBox c = d as ListBox;
            if (c != null)
            {
                if ((bool)e.NewValue)
                {
                    c.SelectionChanged -= ListBoxSelectionChanged;
                    c.SelectionChanged += ListBoxSelectionChanged;
                }
                else
                {
                    c.SelectionChanged -= ListBoxSelectionChanged;
                }
            }
        }

        static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = (ListBox) sender;
            box.ScrollIntoView(box.SelectedItem);
        }

        #endregion

    }
}
