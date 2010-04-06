using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using LinqTwit.Utilities;

namespace LinqTwit.Infrastructure
{
    public class CustomKeyBindingsCollection : ObservableCollection<CustomKeyBinding>
    {
        private UIElement element;
        private object dataContext;

        public UIElement Element
        {
            get 
            {
                return this.element;
            }
            set 
            {
                if (this.element != null)
                {
                    this.element.PreviewTextInput -= ElementPreviewTextInput;
                    this.element.PreviewKeyDown -= ElementOnPreviewKeyDown;
                }
                this.element = value;
                this.element.PreviewTextInput += ElementPreviewTextInput;
                this.element.PreviewKeyDown += ElementOnPreviewKeyDown;
            }
        }

        private void ElementOnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            HandleKeyDown(args);

        }

        private void HandleKeyDown(KeyEventArgs args)
        {
            args.Handled =
                (from binding in this
                 let handled = binding.Handle(args.Key)
                 where handled
                 select handled).FirstOrDefault();
        }


        void ElementPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBoxBase)
            {
                e.Handled = false;
                return;
            }

            HandleTextInput(e);
        }

        private void HandleTextInput(TextCompositionEventArgs e)
        {
            e.Handled =
                (from binding in this 
                 let handled = binding.Handle(e.Text) 
                 where handled 
                 select handled).
                    FirstOrDefault();
        }

        public object DataContext
        {
            get 
            {
                return dataContext;
            }
            set 
            {
                dataContext = value;
                this.ForEach(binding => binding.DataContext = value);
            }
        }

        public static readonly DependencyProperty CustomKeyBindingsProperty =
          DependencyProperty.RegisterAttached(
              "CustomKeyBindings",
              typeof(CustomKeyBindingsCollection),
              typeof(CustomKeyBindingsCollection),
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
