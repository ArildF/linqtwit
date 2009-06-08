using System;
using System.Collections.ObjectModel;
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
    }
}
