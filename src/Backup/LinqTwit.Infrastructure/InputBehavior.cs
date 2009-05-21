using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;

namespace LinqTwit.Infrastructure
{
    public static class InputBehavior
    {
        public static readonly DependencyProperty CustomKeyBindingsProperty =
           DependencyProperty.RegisterAttached(
               "CustomKeyBindings",
               typeof(CustomKeyBindingsCollection),
               typeof(InputBehavior),
               new PropertyMetadata(new CustomKeyBindingsCollection(), CustomKeyBindingsChanged));

        private static CustomKeyBindingsCollection collection;

        private static void CustomKeyBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            CustomKeyBindingsCollection coll = e.NewValue as CustomKeyBindingsCollection;
            if (element != null && coll != null)
            {
                collection = coll;
                coll.Element = element;

                coll.DataContext = element.DataContext;
                element.DataContextChanged += (_1, _2) =>
                    {
                        coll.DataContext = element.DataContext;
                    };
                element.Loaded += new RoutedEventHandler(binding_Loaded);

            }

            
        }

        public static int Whatever
        {
            get { return Whatever; }
        }

        static void binding_Loaded(object sender, RoutedEventArgs e)
        {
        }

        static void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                
            }
        }

        public static void SetCustomKeyBindings(FrameworkElement obj,
                                                  CustomKeyBindingsCollection value)
        {
            obj.SetValue(CustomKeyBindingsProperty, value);
        }

        public static CustomKeyBindingsCollection GetCustomKeyBindings(FrameworkElement obj)
        {
            return (CustomKeyBindingsCollection)obj.GetValue(CustomKeyBindingsProperty);
        }
    }
}
